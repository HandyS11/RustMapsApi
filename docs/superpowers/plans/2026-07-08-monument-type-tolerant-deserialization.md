# Tolerant MonumentType / BiomeType Deserialization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make integer-on-the-wire enum deserialization tolerant so one unknown `MonumentType` (or `BiomeType`) value can never abort a whole `MapInfo` parse, and add the one monument type (`ApartmentsComplex = 560`) currently missing from the enum.

**Architecture:** Add the missing enum member to match the authoritative RustMaps OpenAPI. Add an `Unknown = -1` sentinel to `MonumentType` and `BiomeType`. Introduce a generic `TolerantNumberEnumConverter<TEnum>` that maps any undefined or non-numeric wire value to a supplied fallback instead of throwing, and register it in place of the built-in `JsonNumberEnumConverter<T>` at the single production wiring site (`RustMapsClient`). Update the three test option-helpers to mirror production.

**Tech Stack:** C# / .NET, `System.Text.Json` (with source-gen `JsonSerializerContext`), xUnit + built-in `Assert.*`, NSubstitute (not needed here), Stryker mutation testing.

## Global Constraints

- Library `src/RustMapsApi/RustMapsApi.csproj` multi-targets `netstandard2.0;net10.0` — **all new code must compile for both**. Use only APIs available on `netstandard2.0` (with the project's PolySharp polyfills). The non-generic `Enum.IsDefined(Type, object)` and `Enum.ToObject(Type, object)` are the multi-target-safe choices.
- The library has **no logger** and adds none. No new public dependencies.
- Analyzers are enforced (Microsoft.CodeAnalysis.NetAnalyzers, Roslynator, SonarAnalyzer). New code must be analyzer-clean; unused `using` directives must be removed. Existing `[SuppressMessage]` attributes carry a `Justification`.
- Tests: xUnit with global `using Xunit;`, built-in `Assert.*` (no FluentAssertions/Shouldly). Test naming: `MethodOrSubject_ExpectedBehavior`. Test classes that nest a private source-gen context are declared `public partial class`.
- Solution file is `RustMapsApi.slnx`. Build the library with `dotnet build src/RustMapsApi/RustMapsApi.csproj -c Release` (this compiles **both** TFMs and is the netstandard2.0 gate). Run unit tests with `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj`.
- Conventional-commit messages (`feat:`, `fix:`, `test:`, `chore:`), matching the repo's git history.

**Namespaces for reference (used across tasks):**

- `MonumentType`, `BiomeType`, `MapInfo`, `Monument` → `RustMapsApi.V4.Models`
- `MonumentFilter`, `BiomeFilter` → `RustMapsApi.V4.Requests`
- `ServiceResponse<T>` → `RustMapsApi.Http`
- `RustMapsJsonOptions` → `RustMapsApi.Serialization`
- `RustMapsJsonContextV4` → `RustMapsApi.V4.Serialization`
- `TolerantNumberEnumConverter<TEnum>` (new) → `RustMapsApi.Serialization`

---

### Task 1: Add missing `ApartmentsComplex = 560` monument type

The RustMaps OpenAPI (`https://api.rustmaps.com/swagger/v4-public/swagger.json`) defines `Apartments_Complex = 560`, which is absent from our enum and is the value that crashed the consumer. This task fixes that specific failure on its own (the existing strict converter accepts 560 once the member exists).

**Files:**

- Modify: `src/RustMapsApi/V4/Models/MonumentType.cs` (between `JungleZigguratA = 555` and `CustomMonument = 10000`)
- Test: `tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs`

**Interfaces:**

- Consumes: nothing new.
- Produces: `MonumentType.ApartmentsComplex` (= 560), relied on by tests in Task 3.

- [ ] **Step 1: Write the failing test**

Add this method to the `EnumWireFormatTests` class in `tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs` (after `MonumentType_SerializesAsInteger`):

```csharp
    [Fact]
    public void MonumentType_ApartmentsComplex_DeserializesFromWireValue560()
    {
        var filter = JsonSerializer.Deserialize<MonumentFilter>(
            "{\"type\":560,\"selectionStatus\":\"noPreference\"}", Options());

        Assert.Equal(MonumentType.ApartmentsComplex, filter!.Type);
    }
```

- [ ] **Step 2: Run test to verify it fails**

Run: `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj --filter "FullyQualifiedName~MonumentType_ApartmentsComplex_DeserializesFromWireValue560"`

Expected: FAIL. It will not compile first (`MonumentType.ApartmentsComplex` does not exist) — a compile error is an acceptable red. If you prefer a runtime red, comment the assert's enum reference; otherwise proceed — Step 3 makes it compile and pass.

- [ ] **Step 3: Add the enum member**

In `src/RustMapsApi/V4/Models/MonumentType.cs`, insert between `JungleZigguratA = 555,` and the `CustomMonument` block:

```csharp
    /// <summary>The JungleZigguratA monument type (wire value 555).</summary>
    JungleZigguratA = 555,

    /// <summary>The Apartments Complex monument type (wire value 560).</summary>
    ApartmentsComplex = 560,

    /// <summary>The CustomMonument monument type (wire value 10000).</summary>
    CustomMonument = 10000,
```

- [ ] **Step 4: Run test to verify it passes**

Run: `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj --filter "FullyQualifiedName~MonumentType_ApartmentsComplex_DeserializesFromWireValue560"`

Expected: PASS (1 passed).

- [ ] **Step 5: Commit**

```bash
git add src/RustMapsApi/V4/Models/MonumentType.cs tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs
git commit -m "fix: add MonumentType.ApartmentsComplex (wire value 560) missing from enum"
```

---

### Task 2: Add `Unknown` fallbacks and the `TolerantNumberEnumConverter<TEnum>`

Create the generic tolerant converter and the `Unknown = -1` sentinel on both enums, tested in isolation. Not yet wired into production (Task 3 does that).

**Files:**

- Modify: `src/RustMapsApi/V4/Models/MonumentType.cs` (add `Unknown = -1` at top)
- Modify: `src/RustMapsApi/V4/Models/BiomeType.cs` (add `Unknown = -1` at top)
- Create: `src/RustMapsApi/Serialization/TolerantNumberEnumConverter.cs`
- Create: `tests/RustMapsApi.Tests.Unit/Serialization/TolerantNumberEnumConverterTests.cs`

**Interfaces:**

- Consumes: `MonumentType`, `BiomeType`.
- Produces:
  - `MonumentType.Unknown` (= -1), `BiomeType.Unknown` (= -1)
  - `RustMapsApi.Serialization.TolerantNumberEnumConverter<TEnum>` with `where TEnum : struct, Enum`, constructor `TolerantNumberEnumConverter(TEnum fallback)`, `HandleNull => true`. Consumed by Task 3.

- [ ] **Step 1: Write the failing converter tests**

Create `tests/RustMapsApi.Tests.Unit/Serialization/TolerantNumberEnumConverterTests.cs`:

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.Serialization;

public partial class TolerantNumberEnumConverterTests
{
    private static JsonSerializerOptions Options() => RustMapsJsonOptions.Create(
        ConverterContext.Default,
        new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
        new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));

    [Fact]
    public void MonumentType_UnknownWireValue_MapsToUnknown()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":99999}", Options());

        Assert.Equal(MonumentType.Unknown, monument!.Type);
    }

    [Fact]
    public void MonumentType_KnownWireValue_StillDeserializes()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":45}", Options());

        Assert.Equal(MonumentType.LaunchSite, monument!.Type);
    }

    [Fact]
    public void MonumentType_StringValue_MapsToUnknown()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":\"foo\"}", Options());

        Assert.Equal(MonumentType.Unknown, monument!.Type);
    }

    [Fact]
    public void MonumentType_NullValue_MapsToUnknown()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":null}", Options());

        Assert.Equal(MonumentType.Unknown, monument!.Type);
    }

    [Fact]
    public void BiomeType_UnknownWireValue_MapsToUnknown()
    {
        var filter = JsonSerializer.Deserialize<BiomeFilter>("{\"type\":9999}", Options());

        Assert.Equal(BiomeType.Unknown, filter!.Type);
    }

    [Fact]
    public void MonumentType_KnownValue_SerializesAsInteger()
    {
        var json = JsonSerializer.Serialize(new Monument { Type = MonumentType.LaunchSite }, Options());

        Assert.Contains("\"type\":45", json);
    }

    [Fact]
    public void MonumentType_Unknown_SerializesAsNegativeOne()
    {
        var json = JsonSerializer.Serialize(new Monument { Type = MonumentType.Unknown }, Options());

        Assert.Contains("\"type\":-1", json);
    }

    [JsonSerializable(typeof(Monument))]
    [JsonSerializable(typeof(BiomeFilter))]
    private sealed partial class ConverterContext : JsonSerializerContext;
}
```

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj --filter "FullyQualifiedName~TolerantNumberEnumConverterTests"`

Expected: FAIL — does not compile (`TolerantNumberEnumConverter` and `MonumentType.Unknown`/`BiomeType.Unknown` do not exist yet).

- [ ] **Step 3: Add the `Unknown` sentinel to `MonumentType`**

In `src/RustMapsApi/V4/Models/MonumentType.cs`, insert as the first member (immediately after the opening `{` of the enum, before `NotImplemented = 0`):

```csharp
    /// <summary>An unrecognized monument type not known to this library version. Not part of the
    /// wire protocol; the tolerant-deserialization fallback for any undefined value.</summary>
    Unknown = -1,

    /// <summary>The NotImplemented monument type (wire value 0).</summary>
    NotImplemented = 0,
```

- [ ] **Step 4: Add the `Unknown` sentinel to `BiomeType`**

In `src/RustMapsApi/V4/Models/BiomeType.cs`, insert as the first member (after the opening `{`, before `Snow = 2`):

```csharp
    /// <summary>An unrecognized biome type not known to this library version. Not part of the
    /// wire protocol; the tolerant-deserialization fallback for any undefined value.</summary>
    Unknown = -1,

    /// <summary>The snow (arctic) biome.</summary>
    Snow = 2,
```

- [ ] **Step 5: Create the tolerant converter**

Create `src/RustMapsApi/Serialization/TolerantNumberEnumConverter.cs`:

```csharp
using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RustMapsApi.Serialization;

/// <summary>
/// Converts an integer-on-the-wire enum, mapping any value not defined in
/// <typeparamref name="TEnum"/> to a supplied fallback member instead of throwing, so one
/// unrecognized value never aborts the whole payload. <see cref="Write"/> emits the numeric value.
/// </summary>
/// <typeparam name="TEnum">The integer-backed enum type.</typeparam>
internal sealed class TolerantNumberEnumConverter<TEnum> : JsonConverter<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _fallback;

    /// <summary>Creates the converter with the fallback used for unrecognized values.</summary>
    /// <param name="fallback">The member returned for any value not defined in <typeparamref name="TEnum"/>.</param>
    public TolerantNumberEnumConverter(TEnum fallback) => _fallback = fallback;

    // Ensures Read is invoked for JSON null too, so a null wire value degrades to the fallback
    // instead of the serializer throwing before the converter runs.
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

- [ ] **Step 6: Build the library for both target frameworks**

Run: `dotnet build src/RustMapsApi/RustMapsApi.csproj -c Release`

Expected: Build succeeded, 0 errors, for `netstandard2.0` and `net10.0`.

If the build reports the `BiomeType` `[SuppressMessage(... "CA1027" ...)]` as now unnecessary (analyzer `IDE0079`, because adding `Unknown = -1` disqualifies the power-of-two flags heuristic), remove that attribute block from `src/RustMapsApi/V4/Models/BiomeType.cs`:

```csharp
namespace RustMapsApi.V4.Models;

/// <summary>A Rust biome type. Values match the RustMaps wire protocol.</summary>
public enum BiomeType
{
```

Then re-run the build and confirm it succeeds. If the build did **not** flag it, leave the suppression as-is.

- [ ] **Step 7: Run the converter tests to verify they pass**

Run: `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj --filter "FullyQualifiedName~TolerantNumberEnumConverterTests"`

Expected: PASS (7 passed).

- [ ] **Step 8: Commit**

```bash
git add src/RustMapsApi/V4/Models/MonumentType.cs src/RustMapsApi/V4/Models/BiomeType.cs src/RustMapsApi/Serialization/TolerantNumberEnumConverter.cs tests/RustMapsApi.Tests.Unit/Serialization/TolerantNumberEnumConverterTests.cs
git commit -m "feat: add TolerantNumberEnumConverter and Unknown enum fallbacks"
```

---

### Task 3: Wire the tolerant converter into production and mirror it in test helpers

Replace the strict `JsonNumberEnumConverter<T>` at the single production site and in the three test option-helpers, then add the end-to-end regression test proving one unknown monument no longer sinks a whole `MapInfo`.

**Files:**

- Modify: `src/RustMapsApi/V4/RustMapsClient.cs:23-26` (+ remove unused using at line 4)
- Modify: `tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs:11-14`
- Modify: `tests/RustMapsApi.Tests.Unit/V4/CustomMapSettingsRoundTripTests.cs:12-16` (+ remove unused using at line 2)
- Modify: `tests/RustMapsApi.Tests.Unit/V4/RecordCoverageTests.cs:13-17` (+ remove unused using at line 2)
- Test: `tests/RustMapsApi.Tests.Unit/Serialization/TolerantNumberEnumConverterTests.cs` (add the envelope regression test)

**Interfaces:**

- Consumes: `TolerantNumberEnumConverter<TEnum>`, `MonumentType.Unknown`, `BiomeType.Unknown`, `MonumentType.ApartmentsComplex` (Tasks 1–2).
- Produces: production `RustMapsClient` now deserializes maps tolerantly. No new public surface.

- [ ] **Step 1: Write the failing end-to-end regression test**

Add to `TolerantNumberEnumConverterTests` (from Task 2). It needs `ServiceResponse<MapInfo>` registered, so also add the `using` and the `[JsonSerializable]` line.

Add near the other `using` directives at the top of the file:

```csharp
using RustMapsApi.Http;
```

Add this test method inside the class:

```csharp
    [Fact]
    public void MapInfo_WithUnknownMonument_DeserializesAndKeepsRawImageUrl()
    {
        const string json = """
            {"data":{"rawImageUrl":"https://example/raw.png",
            "monuments":[{"type":99999},{"type":45}]},
            "meta":{"status":"success","statusCode":200}}
            """;

        var envelope = JsonSerializer.Deserialize<ServiceResponse<MapInfo>>(json, Options());

        Assert.NotNull(envelope!.Data);
        Assert.Equal("https://example/raw.png", envelope.Data!.RawImageUrl);
        Assert.Equal(MonumentType.Unknown, envelope.Data.Monuments![0].Type);
        Assert.Equal(MonumentType.LaunchSite, envelope.Data.Monuments[1].Type);
    }
```

Add `ServiceResponse<MapInfo>` to the nested `ConverterContext` so source-gen can resolve it:

```csharp
    [JsonSerializable(typeof(Monument))]
    [JsonSerializable(typeof(BiomeFilter))]
    [JsonSerializable(typeof(ServiceResponse<MapInfo>))]
    private sealed partial class ConverterContext : JsonSerializerContext;
```

- [ ] **Step 2: Run the new test to verify it passes already at the converter level**

Run: `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj --filter "FullyQualifiedName~MapInfo_WithUnknownMonument_DeserializesAndKeepsRawImageUrl"`

Expected: PASS. (This test uses the tolerant `Options()` directly, so it is green as soon as it compiles — it is the regression guard the production wiring must preserve.)

- [ ] **Step 3: Wire the tolerant converter into `RustMapsClient`**

In `src/RustMapsApi/V4/RustMapsClient.cs`, replace lines 23-26:

```csharp
    private readonly JsonSerializerOptions _jsonOptions = RustMapsJsonOptions.Create(
        RustMapsJsonContextV4.Default,
        new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
        new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));
```

Then remove the now-unused `using System.Text.Json.Serialization;` (line 4 — it was only needed for `JsonNumberEnumConverter`). Keep `using System.Text.Json;` (line 3) and `using RustMapsApi.Serialization;` (line 7).

- [ ] **Step 4: Update the `EnumWireFormatTests` option-helper**

In `tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs`, replace the `Options()` body (lines 11-14):

```csharp
    private static JsonSerializerOptions Options() => RustMapsJsonOptions.Create(
        EnumContext.Default,
        new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
        new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));
```

Leave the file's `using System.Text.Json.Serialization;` in place — it is still needed for the `[JsonSerializable]` attributes on `EnumContext`.

- [ ] **Step 5: Update the `CustomMapSettingsRoundTripTests` option-helper**

In `tests/RustMapsApi.Tests.Unit/V4/CustomMapSettingsRoundTripTests.cs`, replace the `Options()` body (lines 12-16):

```csharp
    private static JsonSerializerOptions Options() =>
        RustMapsJsonOptions.Create(
            RustMapsJsonContextV4.Default,
            new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
            new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));
```

Then remove the now-unused `using System.Text.Json.Serialization;` (line 2 — this file has no other `System.Text.Json.Serialization` symbols).

- [ ] **Step 6: Update the `RecordCoverageTests` option-helper**

In `tests/RustMapsApi.Tests.Unit/V4/RecordCoverageTests.cs`, replace the `Options()` body (lines 13-17):

```csharp
    private static JsonSerializerOptions Options() =>
        RustMapsJsonOptions.Create(
            RustMapsJsonContextV4.Default,
            new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
            new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));
```

Then remove the now-unused `using System.Text.Json.Serialization;` (line 2 — this file has no other `System.Text.Json.Serialization` symbols).

- [ ] **Step 7: Build and run the full unit-test suite**

Run: `dotnet build src/RustMapsApi/RustMapsApi.csproj -c Release && dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj`

Expected: Build succeeded (both TFMs, 0 warnings-as-errors). All unit tests PASS, including the pre-existing `EnumWireFormatTests` (known values still serialize as integers) and the new tolerant/regression tests.

- [ ] **Step 8: Commit**

```bash
git add src/RustMapsApi/V4/RustMapsClient.cs tests/RustMapsApi.Tests.Unit/V4/Models/EnumWireFormatTests.cs tests/RustMapsApi.Tests.Unit/V4/CustomMapSettingsRoundTripTests.cs tests/RustMapsApi.Tests.Unit/V4/RecordCoverageTests.cs tests/RustMapsApi.Tests.Unit/Serialization/TolerantNumberEnumConverterTests.cs
git commit -m "fix: use tolerant enum converters so unknown monument types don't fail MapInfo parsing"
```

---

### Task 4: Full verification

Confirm the whole solution builds on both target frameworks and the entire unit suite is green, so the netstandard2.0 constraint and existing behavior are both preserved.

**Files:** none (verification only).

- [ ] **Step 1: Clean build of the library (both TFMs)**

Run: `dotnet build src/RustMapsApi/RustMapsApi.csproj -c Release`

Expected: `Build succeeded.` with 0 errors for `netstandard2.0` and `net10.0`.

- [ ] **Step 2: Run the complete unit-test suite**

Run: `dotnet test tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj -c Release`

Expected: All tests pass, 0 failed. Note the new tests: `MonumentType_ApartmentsComplex_DeserializesFromWireValue560`, the seven `TolerantNumberEnumConverterTests` behaviors, and `MapInfo_WithUnknownMonument_DeserializesAndKeepsRawImageUrl`.

- [ ] **Step 3 (optional but recommended): Mutation-test the converter**

If Stryker is configured for the unit project, run it scoped to the new converter to confirm the branches (defined-number, undefined-number, non-number/null, structured-token skip, write) are covered:

Run: `dotnet stryker --mutate "src/RustMapsApi/Serialization/TolerantNumberEnumConverter.cs"` (from the unit-test project directory, matching how `stryker-config.json` is invoked).

Expected: No surviving mutants in `TolerantNumberEnumConverter.cs`; if any survive, add the missing assertion to `TolerantNumberEnumConverterTests` and re-run.

- [ ] **Step 4: Final confirmation**

Confirm `git status` shows only the intended files changed and all commits from Tasks 1–3 are present:

Run: `git log --oneline -3`

Expected: the three commits from Tasks 1–3, newest first.

---

## Notes / follow-ups (not code tasks)

- The input problem report `docs/development/monument-type-tolerant-deserialization.md` has already been deleted (spec validated).
- `MonumentType.ApartmentsComplex`, `MonumentType.Unknown`, and `BiomeType.Unknown` are additive public-API changes on the `1.0.0-beta` package. If a changelog/release notes file is maintained, add a line noting the tolerant-deserialization behavior and the new members.
- The authoritative OpenAPI for future enum reconciliation is at `https://api.rustmaps.com/swagger/v4-public/swagger.json` (requires a browser `User-Agent` — Cloudflare returns 403 otherwise).
