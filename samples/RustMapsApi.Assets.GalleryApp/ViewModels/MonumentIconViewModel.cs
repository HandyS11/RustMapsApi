using Avalonia.Media;
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

public sealed class MonumentIconViewModel
{
    public MonumentIconViewModel(MonumentAsset asset, IImage image)
    {
        TypeName = asset.MonumentType.ToString();
        AssetName = asset.AssetName;
        Image = image;
    }

    public string TypeName { get; }

    public string AssetName { get; }

    public IImage Image { get; }
}
