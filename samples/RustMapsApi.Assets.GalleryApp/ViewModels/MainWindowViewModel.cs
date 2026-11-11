using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Performance", "CA1812:Avoid uninstantiated internal classes",
    Justification = "Created by the DI container (AddSingleton<MainWindowViewModel> in App).")]
internal sealed class MainWindowViewModel
{
    public MainWindowViewModel(IMonumentAssetSource assets)
    {
        Icons =
        [
            .. assets.AvailableTypes
                .Select(assets.GetAsset)
                .OrderBy(asset => asset.AssetName, StringComparer.Ordinal)
                .Select(asset => new MonumentIconViewModel(asset, SvgImageFactory.Create(asset))),
        ];

        Header = $"{Icons.Count} monument icons";
    }

    public IReadOnlyList<MonumentIconViewModel> Icons { get; }

    public string Header { get; }
}
