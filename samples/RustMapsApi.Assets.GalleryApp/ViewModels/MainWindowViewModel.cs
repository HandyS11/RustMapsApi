using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

public sealed class MainWindowViewModel
{
    public MainWindowViewModel(IMonumentAssetSource assets)
    {
        Icons = assets.AvailableTypes
            .Select(assets.GetAsset)
            .OrderBy(asset => asset.AssetName, StringComparer.Ordinal)
            .Select(asset => new MonumentIconViewModel(asset))
            .ToList();

        Header = $"{Icons.Count} monument icons";
    }

    public IReadOnlyList<MonumentIconViewModel> Icons { get; }

    public string Header { get; }
}
