using Avalonia.Svg.Skia;
using RustMapsApi.V4.Assets;
using Svg;
// Alias needed: the Svg package also declares an unrelated SvgImage type (an SVG image
// element), which otherwise collides with the Avalonia render target of the same name.
using SvgImage = Avalonia.Svg.Skia.SvgImage;

namespace RustMapsApi.Assets.GalleryApp;

/// <summary>Turns a packaged monument SVG into an Avalonia image.</summary>
/// <remarks>
/// The only place that touches Avalonia.Svg.Skia. It reads the SVG from the <em>package</em>
/// (<see cref="MonumentAsset.OpenStream"/>) so the sample proves the package delivers the icon. If the
/// pinned Avalonia.Svg.Skia version exposes a different loader, adjust ONLY this method —
/// everything else binds to Avalonia.Media.IImage and is unaffected.
/// </remarks>
internal static class SvgImageFactory
{
    /// <summary>Loads <paramref name="asset"/>'s SVG into a renderable image.</summary>
    /// <param name="asset">The packaged monument asset to load.</param>
    /// <returns>The image for a gallery tile.</returns>
    /// <remarks>
    /// The returned image is held by a tile for the whole app lifetime (the gallery is static),
    /// so it is intentionally not disposed here; the OS reclaims it on exit.
    /// </remarks>
    public static SvgImage Create(MonumentAsset asset)
    {
        using var stream = asset.OpenStream();
        var document = SvgDocument.Open<SvgDocument>(stream);
        return new SvgImage
        {
            Source = SvgSource.LoadFromSvgDocument(document)
        };
    }
}
