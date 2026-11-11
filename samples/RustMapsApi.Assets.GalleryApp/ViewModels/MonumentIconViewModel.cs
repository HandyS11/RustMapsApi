using Avalonia.Media;
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

internal sealed class MonumentIconViewModel(MonumentAsset asset, IImage image)
{
    public string TypeName { get; } = asset.MonumentType.ToString();

    public string AssetName { get; } = asset.AssetName;

    public IImage Image { get; } = image;
}
