using Avalonia.Svg.Skia;
using RustMapsApi.V4.Assets;
using Svg;
// Alias needed: the Svg package also declares an unrelated SvgImage type (an SVG image
// element), which otherwise collides with the Avalonia render target of the same name.
using SvgImage = Avalonia.Svg.Skia.SvgImage;

namespace RustMapsApi.Assets.GalleryApp;

// The only place that touches Avalonia.Svg.Skia. It reads the SVG from the *package*
// (MonumentAsset.OpenStream) so the sample proves the package delivers the icon. If the
// pinned Avalonia.Svg.Skia version exposes a different loader, adjust ONLY this method —
// everything else binds to Avalonia.Media.IImage and is unaffected.
internal static class SvgImageFactory
{
    public static SvgImage Create(MonumentAsset asset)
    {
        using var stream = asset.OpenStream();
        var document = SvgDocument.Open<SvgDocument>(stream);
        return new SvgImage { Source = SvgSource.LoadFromSvgDocument(document) };
    }
}
