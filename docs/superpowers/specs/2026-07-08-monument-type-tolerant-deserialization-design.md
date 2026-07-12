# Tolerant `MonumentType` / `BiomeType` deserialization — design

- **Date:** 2026-07-08
- **Status:** Approved (pending implementation plan)
- **Source problem:** `docs/development/monument-type-tolerant-deserialization.md` (findings from
  RustPlusBot against a live server; that file is transient and is deleted once this spec is validated)

## Problem

A real `GET map by seed+size` response (HTTP 200, valid body) fails to deserialize because
`monuments[0].type` carries a wire value the strict `JsonNumberEnumConverter<MonumentType>`
rejects. The converter throws a `JsonException`, which propagates uncaught out of
`ResultFactory.FromResponseAsync` (`ResultFactory.cs:31`) and aborts the **entire** `MapInfo`
deserialization — even for consumers that never read `Monuments`. RustPlusBot only needs
`MapInfo.RawImageUrl`, yet a single unknown monument type makes the whole lookup fail and forces a
fallback to a lower-quality map tile.

`BiomeType` has the identical latent bug (also registered as a strict
`JsonNumberEnumConverter<BiomeType>`), though biomes are a far more stable set.

## Root cause

- `MonumentType` (`src/RustMapsApi/V4/Models/MonumentType.cs`) is a plain `int` enum with a fixed
  set of wire values. `Monument.Type` is a **non-nullable** `MonumentType` with no attributes.
- `RustMapsClient.cs:23-26` builds the shared `JsonSerializerOptions` via
  `RustMapsJsonOptions.Create(...)`, registering `JsonNumberEnumConverter<BiomeType>` and
  `JsonNumberEnumConverter<MonumentType>`. The built-in number-enum converter throws on any integer
  not defined in the enum.
- This is the **single** production wiring site. The DI extension
  (`ServiceCollectionExtensions.cs`) only configures the `HttpClient` and delegates to
  `RustMapsClient`, which builds its own options. Other constructions of the converter pair live in
  test helpers.

## OpenAPI reconciliation (identifies the offending value)

The authoritative RustMaps OpenAPI spec was fetched from
`https://api.rustmaps.com/swagger/v4-public/swagger.json` (NSwag-generated; requires a browser
`User-Agent` — Cloudflare returns 403 otherwise) and diffed against our C# enums:

- **`MonumentType` is missing exactly one member: `Apartments_Complex = 560`** — no other drift, no
  renames, no removals. Value 560 sits between `JungleZigguratA = 555` and `CustomMonument = 10000`.
  This is the monument type that crashed RustPlusBot (added to the game after the enum was authored).
- **`BiomeType` is fully in sync** (Snow=2, Desert=4, Forest=8, Tundra=16, Jungle=32). No new
  members required.

## Decisions

| Decision | Choice | Rationale |
|---|---|---|
| Fallback representation | New `Unknown = -1` member per enum | Clear, distinct signal; smaller than exposing a raw int; distinguishable from `NotImplemented = 0` and from an absent field. |
| Scope | Fix **both** `MonumentType` and `BiomeType` | We're writing the first custom converter anyway; making it generic eliminates the whole class of bug in one line each. |
| Capture the offending value | Reconcile against the OpenAPI and add missing members | Authoritative; found `ApartmentsComplex = 560` directly instead of guessing from one live map. |
| Tolerance breadth | Fall back on **any** unmappable token (undefined ints *and* non-numeric tokens) | Fully honors "one field never aborts the map." |

**Explicitly rejected:** Approach 2 (`Monument.TypeRaw` int) — larger public API surface; adding
logging to the library (it targets `netstandard2.0` and has no logger); auto-generating enums from
OpenAPI; changing the `RustMapsJsonOptions.Create` signature.

## Design

### Part A — enum reconciliation (the specific fix)

Add to `MonumentType.cs`, in numeric order between `JungleZigguratA = 555` and
`CustomMonument = 10000`:

```csharp
/// <summary>The Apartments Complex monument type (wire value 560).</summary>
ApartmentsComplex = 560,
```

`BiomeType` gets no new wire members.

### Part B — graceful degradation (the general fix)

**1. Fallback members.** Add a sentinel to both enums, documented as *not* a wire value:

```csharp
// MonumentType.cs
/// <summary>An unrecognized monument type not known to this library version.
/// Not part of the wire protocol; the tolerant-deserialization fallback for
/// any value this version does not define.</summary>
Unknown = -1,

// BiomeType.cs — same, for biome types
Unknown = -1,
```

`BiomeType` keeps its existing `[SuppressMessage(CA1027)]`; `Unknown = -1` is a single-value
sentinel, consistent with that suppression's justification, and does not make it a flags enum.

**2. Generic converter** — new file `src/RustMapsApi/Serialization/TolerantNumberEnumConverter.cs`
(the repo's first custom `JsonConverter<T>`; must compile for both `netstandard2.0` and `net10.0`
and coexist with the source-gen `TypeInfoResolver`, exactly as the built-in converters do today):

```csharp
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RustMapsApi.Serialization;

/// <summary>
/// Converts an integer-on-the-wire enum, mapping any value not defined in
/// <typeparamref name="TEnum"/> to a supplied fallback member instead of throwing, so one
/// unrecognized value never aborts the whole payload. Write emits the numeric value.
/// </summary>
internal sealed class TolerantNumberEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _fallback;

    public TolerantNumberEnumConverter(TEnum fallback) => _fallback = fallback;

    // Ensures Read is invoked for JSON null too, so a null wire value degrades to the
    // fallback instead of the serializer throwing before the converter runs.
    public override bool HandleNull => true;

    public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number
            && reader.TryGetInt32(out var value)
            && Enum.IsDefined(typeof(TEnum), value))
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        // Any undefined or non-numeric value degrades to the fallback rather than throwing.
        if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
        {
            reader.Skip();
        }

        return _fallback;
    }

    public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        => writer.WriteNumberValue(Convert.ToInt32(value, CultureInfo.InvariantCulture));
}
```

Behavior notes:

- `Enum.IsDefined(typeof(TEnum), value)` + `Enum.ToObject(...)` work on both target frameworks for
  int-backed enums (the boxing cost is negligible — a few dozen monuments per map, not a hot path).
- Structured tokens (`StartObject`/`StartArray`) are consumed via `reader.Skip()` before returning
  the fallback, so the reader stays correctly positioned. Scalar non-number tokens (string/null/
  bool) need no skip.
- `Write` emits the numeric value; `Unknown` round-trips to `-1`. Acceptable because `Monument` is a
  read model, never a request body — `Write` for these enums is exercised only in round-trip tests.

**3. Wiring** — the single production site, `RustMapsClient.cs:23-26`:

```csharp
private readonly JsonSerializerOptions _jsonOptions = RustMapsJsonOptions.Create(
    RustMapsJsonContextV4.Default,
    new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
    new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));
```

Drop the now-unused `using System.Text.Json.Serialization;` from `RustMapsClient.cs` if nothing else
in the file needs it (analyzers are strict). `RustMapsJsonOptions.Create` is unchanged.

## Testing

xUnit + built-in `Assert.*` + nested source-gen `JsonSerializerContext`, matching
`tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs`:

1. **Regression guard for the reported failure:** `type: 560` deserializes to
   `MonumentType.ApartmentsComplex`.
2. **Tolerant fallback:** an unknown int (e.g. `99999`) → `MonumentType.Unknown`, no throw. Same for
   `BiomeType.Unknown`.
3. **Core regression:** a full `ServiceResponse<MapInfo>` JSON literal whose monument carries an
   unknown `type` deserializes successfully **and** `RawImageUrl` is still populated — proving one
   bad monument no longer sinks the whole lookup.
4. **Non-numeric tolerance:** `type: "foo"` / `type: null` → `Unknown` (documents the chosen breadth).
5. **Known values unchanged:** defined values still round-trip as integers (existing assertions in
   `EnumWireFormatTests` continue to hold).
6. Update the test `Options()` helpers that construct `new JsonNumberEnumConverter<…>()`
   (`EnumWireFormatTests`, `RustMapsJsonOptionsTests`, and any others) to use
   `TolerantNumberEnumConverter<…>(fallback)` so tests mirror production.

Mutation testing (Stryker) is in use; the converter's branches (number/defined, number/undefined,
non-number, structured-token skip) should each be covered so mutants are killed.

## Files touched

| File | Change |
|---|---|
| `src/RustMapsApi/V4/Models/MonumentType.cs` | Add `Unknown = -1` and `ApartmentsComplex = 560` |
| `src/RustMapsApi/V4/Models/BiomeType.cs` | Add `Unknown = -1` |
| `src/RustMapsApi/Serialization/TolerantNumberEnumConverter.cs` | New generic converter |
| `src/RustMapsApi/V4/RustMapsClient.cs` | Swap the two converter registrations; tidy usings |
| `tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs` (+ new/adjacent test file) | New tolerant-deserialization + regression tests; update `Options()` helpers |
| `tests/RustMapsApi.Tests.Unit/Serialization/RustMapsJsonOptionsTests.cs` | Update `Options()` helper to tolerant converter |

## Non-goals

- No `Monument.TypeRaw` / raw-int preservation.
- No logging added to the library.
- No change to `RustMapsJsonOptions.Create`'s signature or the catch-all string-enum behavior.
- No auto-generation of enums from OpenAPI (manual reconciliation when needed).

## Follow-ups / cleanup

- Delete `docs/development/monument-type-tolerant-deserialization.md` once this spec is validated.
- Consider a `reference` note that the OpenAPI spec is at
  `https://api.rustmaps.com/swagger/v4-public/swagger.json` (browser `User-Agent` required) for
  future enum reconciliation.
- Public API additions (`Unknown` members, `ApartmentsComplex`) are additive on a `1.0.0-beta`
  package; note them in release notes / changelog if one is maintained.
