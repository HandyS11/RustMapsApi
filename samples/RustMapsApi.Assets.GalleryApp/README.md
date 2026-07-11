# Monument icon gallery (Avalonia)

A cross-platform desktop app that renders **every** monument icon bundled in
[`RustMapsApi.Assets`](../../src/RustMapsApi.Assets/README.md) — no API key, no network.

It builds a `ServiceCollection`, calls `AddRustMapsAssets()`, injects `IMonumentAssetSource`,
enumerates `AvailableTypes`, and renders each icon (SVG) with
[Avalonia.Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia).

## Run

```bash
dotnet run --project samples/RustMapsApi.Assets.GalleryApp
```

A window opens with a scrollable grid of monument icons, each labelled with its `MonumentType`.

<!-- Optional: drop a screenshot.png beside this file and reference it here. -->
