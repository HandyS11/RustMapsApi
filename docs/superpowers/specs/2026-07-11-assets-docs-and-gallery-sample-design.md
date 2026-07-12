# `RustMapsApi.Assets` — documentation + icon-gallery sample — design

- **Date:** 2026-07-11
- **Status:** Approved (pending implementation plan)
- **Goal:** Surface the new `RustMapsApi.Assets` package across the project's documentation, and add
  a runnable Avalonia desktop sample that renders the bundled monument icons — so a reader arriving
  at the repo, the docs site, or the samples folder discovers the second package and can *see* it
  work in seconds.

## Problem

`RustMapsApi.Assets` shipped (monument icon SVGs mapped by `MonumentType`, resolved offline with no
network call). But the surrounding materials still describe a single-package project:

- The root [`README.md`](../../../README.md) mentions only the client — no packages table, no Assets.
- The docs site describes **"One package"**: [`docs/index.md`](../../../docs/index.md) and
  [`docs/articles/introduction.md`](../../../docs/articles/introduction.md) list only `RustMapsApi`;
  there is **no Assets guide article** paralleling `client.md`.
- The samples ([`samples/`](../../../samples)) are three console apps over `IRustMapsClient`. None
  touches the Assets package, and in particular **nothing exercises the injectable
  `IMonumentAssetSource` / `AddRustMapsAssets()` DI path** — the console DI sample only wires the
  client.
- The package's real payoff is *visual* (rendering an icon), which no console sample can convey.

## Goals / non-goals

### Goals

1. Document `RustMapsApi.Assets` everywhere a reader would look: root README, docs landing page,
   introduction, a dedicated Assets guide article, and the samples pages.
2. Add one cross-platform Avalonia desktop sample that renders **every** available monument icon in
   a gallery, offline, with **no API key**, wired through the package's **DI** API.
3. Keep the whole solution green under the repo's existing gates (warnings-as-errors, analyzer
   stack, `jb cleanupcode` zero-diff, cross-OS CI, Sonar/coverage/mutation).

### Non-goals

- No live map lookup / network / API-key flow in the sample (offline gallery only).
- No WPF or Windows-only project (would break the cross-platform CI build).
- No changes to the `RustMapsApi.Assets` package code itself — this is docs + a consumer sample.
- No new test project for the sample (samples are untested by design; see gate analysis below).

## Deliverable 1 — Avalonia icon-gallery sample

### Project

- **Name:** `RustMapsApi.Assets.GalleryApp`, at `samples/RustMapsApi.Assets.GalleryApp/`.
- **Framework:** `net10.0`, `OutputType=WinExe` (avoids a console window on Windows; treated as a
  normal executable elsewhere). `IsPackable=false` (inherited).
- Registered in [`RustMapsApi.slnx`](../../../RustMapsApi.slnx) under the `/samples/` folder.
- **Project references:** a single `ProjectReference` to `src/RustMapsApi.Assets` — it already
  references `src/RustMapsApi`, so the models (`MonumentType`) and the Assets API arrive
  transitively.
- **Package references:** the Avalonia packages below, plus **`Microsoft.Extensions.DependencyInjection`**
  (already centrally versioned). The Assets package only references the DI *abstractions*; the app
  needs the concrete package to call `BuildServiceProvider()`.

### Behaviour (offline, no key, no network)

1. On startup the composition root builds a `ServiceCollection`, calls **`AddRustMapsAssets()`**, and
   resolves the main view model, which depends on **`IMonumentAssetSource`**.
2. The view model enumerates `IMonumentAssetSource.AvailableTypes`, calls `GetAsset(type)` for each,
   and turns each `MonumentAsset` into a tile view model exposing the monument type name, the CDN
   `AssetName`, and an image built from the asset's SVG.
3. The window shows the tiles in a scrollable `WrapPanel` grid, with a header line reporting coverage
   (e.g. *"N monument icons"*). Purely presentational; the data never changes after load.

### SVG rendering

Avalonia cannot render SVG natively (same as WPF). Use **`Avalonia.Svg.Skia`** to turn each asset's
SVG into a bound image. The asset exposes both `OpenStream()` and `GetSvg()`; the exact
`Avalonia.Svg.Skia` entry point (loading an `SvgImage`/`SvgSource` from the SVG string or stream) is
pinned during implementation against the chosen package version. Rendering is isolated in one small
helper (e.g. `SvgImageFactory`) so the view models stay free of rendering-library types where
practical.

### Files

```text
samples/RustMapsApi.Assets.GalleryApp/
  RustMapsApi.Assets.GalleryApp.csproj
  Program.cs                       # BuildAvaloniaApp().StartWithClassicDesktopLifetime(args)
  App.axaml / App.axaml.cs         # Application, FluentTheme, DI composition root, sets MainWindow
  MainWindow.axaml / .axaml.cs     # ScrollViewer + ItemsControl(WrapPanel) of icon tiles
  ViewModels/
    MainWindowViewModel.cs         # IReadOnlyList<MonumentIconViewModel>, header text
    MonumentIconViewModel.cs       # TypeName, AssetName, Image
  SvgImageFactory.cs               # MonumentAsset/SVG -> Avalonia image (Avalonia.Svg.Skia)
  README.md                        # what it shows + run command (+ screenshot placeholder)
```

### Central package management

Add to [`Directory.Packages.props`](../../../Directory.Packages.props) (under a new `Samples (UI)`
group), versions pinned to the latest stable compatible with `net10.0` at implementation time:

- `Avalonia`
- `Avalonia.Desktop`
- `Avalonia.Themes.Fluent`
- `Avalonia.Svg.Skia`

## Deliverable 2 — Documentation

- **[`README.md`](../../../README.md):**
  - Add a **Packages** table listing both packages with NuGet badges.
  - Add `RustMapsApi.Assets` to the **Install** section.
  - Add a short **Monument icons** section: one-line what/why, install, a single `TryGetAsset`
    snippet, and a link to the gallery sample.
- **[`docs/index.md`](../../../docs/index.md):**
  - Add the `RustMapsApi.Assets` row to the **Packages** table.
  - Add a **🖼️ Monument icons** feature card linking the new Assets guide.
- **[`docs/articles/introduction.md`](../../../docs/articles/introduction.md):**
  - Replace the "One package" heading/table with a two-row **Packages** table.
  - Add a "What you can do" bullet for monument icons.
- **[`docs/articles/assets.md`](../../../docs/articles/assets.md) (new guide):** parallels
  `client.md`. Covers: install; the `MonumentAssets` static API (`TryGetAsset` / `GetAsset` /
  `HasAsset` / `AvailableTypes`); `MonumentAsset` members (`AssetName`, `FileName`, `MediaType`,
  `SourceUri`, `OpenStream` / `GetBytes` / `GetSvg`); the DI path (`AddRustMapsAssets` →
  `IMonumentAssetSource`); which types have no icon; and rendering SVG in WPF and Avalonia (linking
  the gallery sample). Uses `xref:` links to the API reference like the other articles.
- **[`docs/articles/toc.yml`](../../../docs/articles/toc.yml):** add **Assets** under **Guides**.
- **[`docs/articles/samples.md`](../../../docs/articles/samples.md)** and
  **[`samples/README.md`](../../../samples/README.md):** add a **Monument icon gallery (Avalonia)**
  section (what it shows + `dotnet run` command); update the "two console apps" framing to include
  the new desktop sample.

## CI / gates — how the sample stays green

The global props and CI checks are the hard part of this work, not the UI itself.

- **`GenerateDocumentationFile` (global true):** a UI project has public `App` / `Window` / view-model
  types → CS1591. Override `GenerateDocumentationFile=false` in the sample csproj.
- **`TreatWarningsAsErrors` (global true) + analyzers (NetAnalyzers `latest-all`, Roslynator,
  Sonar):** Avalonia XAML-generated code and UI patterns can trip rules. First choice: keep
  warnings-as-errors and add **targeted `NoWarn`** for the specific, identified Avalonia/XAML/analyzer
  diagnostics. Fallback, scoped to this one project only if the noise can't be pinned cleanly:
  `TreatWarningsAsErrors=false`. (Precedent: the standalone console sample already scopes `NoWarn`
  for S1075.)
- **`jb cleanupcode` zero-diff (CI):** commit `.axaml`/`.cs` pre-formatted; run
  `dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"` locally and confirm
  `git diff` is empty before committing.
- **Cross-OS build (ubuntu + windows):** Avalonia builds on both from NuGet with no native SDK; CI
  only *builds* samples (never runs them), so no display is needed.
- **Coverage / Stryker mutation:** samples are not referenced by any test project, so coverlet does
  not load them and Stryker's solution mode does not mutate them. Confirmed by the existing three
  samples keeping CI green.
- **Sonar:** already excludes `**/samples/**` (analysis and coverage) via `Sonar.yml`.

## Verification

- `dotnet build RustMapsApi.slnx -c Release` clean (warnings-as-errors) on this machine.
- `dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"` → empty `git diff`.
- `dotnet test RustMapsApi.slnx` still green (sample adds no tests; nothing regresses).
- DocFX docs build succeeds with the new `assets.md` in the TOC.
- Manual: launch the gallery on a machine with a display and confirm icons render. **This
  environment is headless**, so the live launch + README screenshot is a manual step left to the
  maintainer; automated verification stops at build + format + test + docs build.

## Risks

- **Avalonia ↔ analyzer/format friction** is the main risk; the `NoWarn`/format steps above are the
  mitigation, with a scoped `TreatWarningsAsErrors=false` as the escape hatch.
- **`Avalonia.Svg.Skia` API shape** varies across versions; isolating it in `SvgImageFactory` keeps
  any churn to one file.
- **NuGet restore of Avalonia packages** must succeed in the build environment; if offline restore
  is unavailable here, build verification is deferred to CI and called out explicitly rather than
  claimed.
