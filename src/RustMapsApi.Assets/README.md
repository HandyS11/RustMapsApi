# RustMapsApi.Assets

Monument icon artwork (SVG) for the [RustMaps API](https://rustmaps.com), mapped by `MonumentType`.
A companion to [`RustMapsApi`](https://www.nuget.org/packages/RustMapsApi): given a monument from an
API response, get its icon with no network call.

## Install

```bash
dotnet add package RustMapsApi.Assets
```

## Usage

```csharp
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
{
    // asset.AssetName  -> "Nuclear_Missile_Silo"
    // asset.FileName   -> "Nuclear_Missile_Silo.svg"
    // asset.MediaType  -> "image/svg+xml"
    // asset.SourceUri  -> https://content.rustmaps.com/assets/Nuclear_Missile_Silo.svg
    string svg = asset.GetSvg();       // markup
    byte[] bytes = asset.GetBytes();   // raw bytes
    using Stream stream = asset.OpenStream();
}
```

Types with no published icon (terrain such as `Mountain*`/`Lake*`, and the
`Unknown`/`NotImplemented`/`CustomMonument` sentinels) return `false` from `TryGetAsset`.
`GetAsset` throws `KeyNotFoundException` for those; `HasAsset` and `AvailableTypes` let you
query coverage.

### WPF

WPF cannot render SVG natively. Feed `OpenStream()` to an SVG renderer such as
[SharpVectors](https://www.nuget.org/packages/SharpVectors.Wpf) or
[SkiaSharp](https://www.nuget.org/packages/SkiaSharp), e.g. with SharpVectors:

```csharp
using var stream = MonumentAssets.GetAsset(monument.Type).OpenStream();
var settings = new WpfDrawingSettings();
using var reader = new FileSvgReader(settings);
DrawingGroup drawing = reader.Read(stream);
var image = new DrawingImage(drawing); // bind to an <Image Source="..." />
```

### Dependency injection (optional)

```csharp
services.AddRustMapsAssets();           // registers IMonumentAssetSource
```

## Attribution

The monument icon artwork is © [RustMaps](https://rustmaps.com) and served from their public CDN
(`content.rustmaps.com`). This package redistributes those icons unmodified for convenience alongside
the RustMaps API. All rights to the artwork remain with RustMaps. The package code is MIT-licensed.
