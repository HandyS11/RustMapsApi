using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

public sealed class MonumentIconViewModel
{
    public MonumentIconViewModel(MonumentAsset asset)
    {
        TypeName = asset.MonumentType.ToString();
        AssetName = asset.AssetName;
    }

    public string TypeName { get; }

    public string AssetName { get; }
}
