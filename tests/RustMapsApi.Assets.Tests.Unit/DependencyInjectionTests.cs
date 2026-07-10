using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddRustMapsAssets_ResolvesSource_ThatDelegatesToStaticApi()
    {
        var provider = new ServiceCollection()
            .AddRustMapsAssets()
            .BuildServiceProvider();

        var source = provider.GetRequiredService<IMonumentAssetSource>();

        Assert.True(source.HasAsset(MonumentType.Outpost));
        Assert.True(source.TryGetAsset(MonumentType.Outpost, out var asset));
        Assert.Equal("Outpost", asset.AssetName);
        Assert.False(source.HasAsset(MonumentType.Mountain1));
        Assert.NotEmpty(source.AvailableTypes);
    }
}
