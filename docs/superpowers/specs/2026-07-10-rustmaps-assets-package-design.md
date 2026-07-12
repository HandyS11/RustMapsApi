# `RustMapsApi.Assets` — monument icon package — design

- **Date:** 2026-07-10
- **Status:** Approved (pending implementation plan)
- **Goal:** Ship a new NuGet package that bundles RustMaps' monument icon artwork (SVG) and maps
  each `MonumentType` from an API response to its icon, so consumers (WPF, web, anything) can render
  a monument's icon straight from a `MapInfo`/`Monument` response with no network call.

## Problem

RustMaps serves a per-monument icon set from its public CDN (`https://content.rustmaps.com/assets/<Name>.svg`,
e.g. `Cave.svg`, `Nuclear_Missile_Silo.svg`). These icons are displayed on every public map, but:

- The **asset list is not published** — there is no index/manifest endpoint. The set must be
  discovered by probing.
- The **CDN file names do not match** the `MonumentType` enum names. They use RustMaps' canonical
  `Title_Case_With_Underscores` monument names, with real irregularities (see below).
- Multiple enum members collapse onto **one** icon (all `Cave*` variants → `Cave.svg`).

A consumer holding a `Monument.Type` (a `MonumentType`) from an API response therefore has no simple,
offline way to obtain the corresponding icon. This package provides that mapping plus the vendored art.

## Discovery findings (empirical, 2026-07-10)

Probing the CDN with a browser `User-Agent` established:

- Naming is **mostly** `Title_Case_With_Underscores`, but **not derivable algorithmically** —
  confirmed irregularities include:
  - `StablesA`/`StablesB` → **`Large_Barn.svg`** (not "Stables")
  - `JungleZigguratA` → **`Ziggurat.svg`** (not "Jungle_Ziggurat")
  - `MilitaryTunnels` → **`Military_Tunnels.svg`** (plural)
  - `WaterTreatment` → **`Water_Treatment.svg`** (not "Water_Treatment_Plant")
  - `Underwater*` → both `Underwater_Lab.svg` and `Underwater.svg` exist
- Variants collapse to a base icon: `Cave*`→`Cave`, `Iceberg*`→`Iceberg`, `Swamp*`→`Swamp`,
  `HarborSmall`/`HarborLarge`→`Harbor`, `SulfurQuarry`→`Sulfur_Quarry`, `StoneQuarry`→`Stone_Quarry`, …
- Confirmed-present base icons include (non-exhaustive): `Cave`, `Airfield`, `Lighthouse`,
  `Supermarket`, `Outpost`, `Powerplant`, `Harbor`, `Iceberg`, `Junkyard`, `Excavator`, `Trainyard`,
  `Gasstation`, `Radtown`, `Swamp`, `Launch_Site`, `Bandit_Town`, `Sphere_Tank`, `Water_Treatment`,
  `Sewer_Branch`, `Satellite_Dish`, `Military_Tunnels`, `Fishing_Village`, `Arctic_Research_Base`,
  `Large_Barn`, `Military_Base`, `Water_Well`, `Power_Substation`, `Powerline`, `Jungle_Ruin`,
  `Ziggurat`, `Tunnel_Entrance`, `Underwater_Lab`, `Nuclear_Missile_Silo`.
- Not yet resolved on first-pass probing (need more sleuthing during implementation): `OilrigLarge`/
  `OilrigSmall`, `HqmQuarry`, `FerryTerminal1`, `ApartmentsComplex`.
- `Mountain*` returns 404 on every convention → pure-terrain types likely have **no icon**.

**Implication:** the mapping must be an explicit, hand-curated, CDN-verified table. Coverage is
partial by nature — some `MonumentType` members (the `NotImplemented`/`Unknown`/`CustomMonument`
sentinels and any genuinely icon-less terrain such as `Mountain*`) have no asset and are simply
absent from the map. The exact assetless set is finalized during implementation and reported.

## Decisions (from brainstorming)

| Question | Decision |
| --- | --- |
| Asset format | **SVG only**, embedded as assembly resources. No PNG/XAML — consumers bring their own renderer. |
| Package boundary | **New package `RustMapsApi.Assets`** with a `ProjectReference` to `RustMapsApi` (→ NuGet dependency), keying off the real `MonumentType`. |
| Return shape | **Try + Get pair** (BCL idiom), **not** the HTTP-coupled `Result<T>`. |
| Mapping storage | **Hand-written explicit map** (`switch` expression) + embedded SVGs. |
| Licensing | Ship the art (owner's decision: icons are already public on RustMaps' CDN); include a short NOTICE crediting RustMaps as the source. |
| Delivery | **Vendored** SVGs (downloaded into the repo, embedded), not runtime-fetched. |

## Architecture

### Project & packaging

- New project `src/RustMapsApi.Assets/RustMapsApi.Assets.csproj`.
  - `TargetFrameworks`: `netstandard2.0;net10.0` (match core).
  - `IsPackable=true`, `PackageId=RustMapsApi.Assets`, MIT, `PackageIcon=icon.png`,
    `PackageReadmeFile=README.md`, tags mirroring core plus `icons;svg`.
  - `<ProjectReference Include="../RustMapsApi/RustMapsApi.csproj" />` (becomes a package dependency).
  - `<EmbeddedResource Include="Assets/*.svg" />`.
- Namespace `RustMapsApi.V4.Assets` (mirrors `RustMapsApi.V4.Models`).
- Wire the new src project **and** the new unit-test project into `RustMapsApi.slnx`.

### Public API surface

- **`MonumentAsset`** — immutable value describing one resolved asset:
  - `MonumentType MonumentType { get; }` — the requested type.
  - `string AssetName { get; }` — CDN base name, e.g. `"Nuclear_Missile_Silo"`.
  - `string FileName { get; }` — `AssetName + ".svg"`.
  - `string MediaType { get; }` — constant `"image/svg+xml"`.
  - `Uri SourceUri { get; }` — canonical CDN URL, for provenance/attribution.
  - Content access: `Stream OpenStream()` (fresh stream over the embedded bytes each call),
    `byte[] GetBytes()`, `string GetSvg()` (UTF-8 markup).
- **`MonumentAssets`** — static entry point:
  - `bool TryGetAsset(MonumentType type, out MonumentAsset asset)` — **primary**; `false` when the
    type has no asset. Allocation-free, no exception.
  - `MonumentAsset GetAsset(MonumentType type)` — convenience; throws `KeyNotFoundException` for
    assetless types (BCL-idiomatic, matching the `Dictionary` indexer; no bespoke exception type).
  - `bool HasAsset(MonumentType type)`.
  - `IReadOnlyCollection<MonumentType> AvailableTypes { get; }`.
- **Optional DI sugar** — `IMonumentAssetSource` (instance mirror of the static API) +
  `AddRustMapsAssets()` extension in the `Microsoft.Extensions.DependencyInjection` namespace, for
  consumers who prefer injection/mocking. The static API remains primary (the lookup is pure &
  stateless).

### Mapping & resolution

- The `MonumentType → assetName` relation is a `switch` expression (works on `netstandard2.0`;
  `FrozenDictionary` is **not** available there). Variants map to their base asset name.
- `AvailableTypes` is a static readonly set derived from the mapped members.
- Resolution: map `MonumentType` → `assetName` → open the embedded resource
  `RustMapsApi.Assets.Assets.<assetName>.svg` via `Assembly.GetManifestResourceStream`.

### Discovery (implementation-time, not shipped)

A throwaway script (in `tmp/`) that, per distinct monument, probes candidate CDN names with a
browser `User-Agent`, confirms a `200`, downloads the SVG into `src/RustMapsApi.Assets/Assets/`, and
emits the resolved `MonumentType → assetName` pairs. The SVGs and the finished map are committed; the
script is not. Final coverage (including the definitive assetless set) is reported.

## Testing

New `tests/RustMapsApi.Assets.Tests.Unit` (offline, deterministic):

- Every mapped `MonumentType` resolves to a **non-empty** SVG stream.
- Every map entry has a matching embedded resource (**no dangling names**).
- Every embedded SVG is referenced by ≥1 map entry (**no orphan resources**).
- Content looks like SVG (starts with `<svg`/`<?xml`).
- Assetless sentinels (`NotImplemented`, `Unknown`, `CustomMonument`) → `TryGetAsset` false /
  `GetAsset` throws; `HasAsset` false.
- `FileName`, `MediaType`, `SourceUri` correctness for a sampled asset.
- `AvailableTypes` matches the map's key set.

Optional (nice-to-have) live-gated Integration test mirroring the existing `RUSTMAPS_API_KEY`-style
skip pattern: `HEAD` each `SourceUri` against the CDN and assert `200`, to catch upstream drift.
Skipped offline.

## Attribution & docs

- Package `README.md` (embedded as `PackageReadmeFile`): install, a 3-line usage snippet, and a WPF
  snippet showing `OpenStream()` fed to a third-party SVG renderer (WPF cannot render SVG natively).
- A short NOTICE line crediting RustMaps as the source of the icon artwork.
- No WPF **sample project** (Windows-only, heavy) — a doc snippet only.

## Out of scope (YAGNI)

- PNG/XAML/rasterized formats.
- Runtime CDN downloader / caching client.
- Biome icons or any non-monument assets.
- A WPF sample project.

## Open items for implementation

- Resolve the four unconfirmed names (`OilrigLarge`/`OilrigSmall`, `HqmQuarry`, `FerryTerminal1`,
  `ApartmentsComplex`) and finalize the assetless-type list.
- Decide `KeyNotFoundException` vs a dedicated `MonumentAssetNotFoundException` for `GetAsset`.
