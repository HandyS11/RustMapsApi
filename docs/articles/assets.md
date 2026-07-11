# Assets

[`RustMapsApi.Assets`](https://www.nuget.org/packages/RustMapsApi.Assets) bundles RustMaps' monument
icon artwork (SVG) and maps each [`MonumentType`](xref:RustMapsApi.V4.Models.MonumentType) to its
icon. Given a monument from an API response, get its icon with **no network call**.

## Install

```bash
dotnet add package RustMapsApi.Assets
```

## Resolve an icon

[`MonumentAssets`](xref:RustMapsApi.V4.Assets.MonumentAssets) is the static entry point. Prefer
`TryGetAsset` — it never throws:

```csharp
using RustMapsApi.V4.Assets;

if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
{
    string svg      = asset.GetSvg();      // markup
    byte[] bytes    = asset.GetBytes();    // raw bytes
    using Stream s  = asset.OpenStream();  // caller disposes
}
```

| Member | Purpose |
| --- | --- |
| `TryGetAsset(type, out asset)` | `true` + the asset when an icon exists; otherwise `false`. |
| `GetAsset(type)` | The asset, or throws `KeyNotFoundException` when none exists. |
| `HasAsset(type)` | Whether an icon exists for the type. |
| `AvailableTypes` | Every `MonumentType` that has an icon. |

## The `MonumentAsset`

A resolved [`MonumentAsset`](xref:RustMapsApi.V4.Assets.MonumentAsset) describes one icon:

| Member | Example |
| --- | --- |
| `MonumentType` | `MonumentType.LaunchSite` |
| `AssetName` | `"Launch_Site"` |
| `FileName` | `"Launch_Site.svg"` |
| `MediaType` | `"image/svg+xml"` |
| `SourceUri` | `https://content.rustmaps.com/assets/Launch_Site.svg` |
| `OpenStream()` | A readable stream over the embedded SVG (you dispose it). |
| `GetBytes()` | The SVG as `byte[]`. |
| `GetSvg()` | The SVG markup as a `string`. |

## Coverage

Not every `MonumentType` has an icon. Terrain types (`Mountain*`, `Lake*`) and the `Unknown` /
`NotImplemented` / `CustomMonument` sentinels return `false` from `TryGetAsset` (and `GetAsset`
throws for them). Use `HasAsset` or enumerate `AvailableTypes`:

```csharp
foreach (var type in MonumentAssets.AvailableTypes)
    Console.WriteLine($"{type} -> {MonumentAssets.GetAsset(type).FileName}");
```

## From an API response

Every `MonumentType` comes straight off a map's monuments, so an icon is one lookup away:

```csharp
var result = await client.GetMapByIdAsync(mapId);
foreach (var monument in result.Data?.Monuments ?? [])
{
    if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
        File.WriteAllBytes(asset.FileName, asset.GetBytes());
}
```

## Dependency injection

Register [`IMonumentAssetSource`](xref:RustMapsApi.V4.Assets.IMonumentAssetSource) — the same
surface as the static class, injectable — with `AddRustMapsAssets`:

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddRustMapsAssets();  // registers IMonumentAssetSource (singleton)
```

```csharp
public sealed class IconService(IMonumentAssetSource assets)
{
    public string? Svg(MonumentType type) =>
        assets.TryGetAsset(type, out var asset) ? asset.GetSvg() : null;
}
```

## Rendering SVG

The bundled art is SVG; neither WPF nor Avalonia renders SVG natively, so feed the asset to an SVG
renderer.

### Avalonia

The runnable [icon gallery sample](https://github.com/HandyS11/RustMapsApi/tree/develop/samples/RustMapsApi.Assets.GalleryApp)
renders every icon with [Avalonia.Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia):

```csharp
using Avalonia.Svg.Skia;
using Svg;

using var stream = MonumentAssets.GetAsset(type).OpenStream();
var document = SvgDocument.Open<SvgDocument>(stream);
var image = new SvgImage { Source = SvgSource.LoadFromSvgDocument(document) };
// bind image to <Image Source="..." />
```

### WPF

Feed `OpenStream()` to a renderer such as
[SharpVectors](https://www.nuget.org/packages/SharpVectors.Wpf):

```csharp
using var stream = MonumentAssets.GetAsset(type).OpenStream();
var reader = new FileSvgReader(new WpfDrawingSettings());
var image = new DrawingImage(reader.Read(stream)); // bind to <Image Source="..." />
```

## Attribution

The monument icon artwork is © [RustMaps](https://rustmaps.com), served from their public CDN
(`content.rustmaps.com`) and redistributed unmodified for convenience. All rights to the artwork
remain with RustMaps; the package code is MIT-licensed.
