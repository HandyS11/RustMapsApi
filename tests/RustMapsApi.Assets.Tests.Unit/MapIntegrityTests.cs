using System.Reflection;
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class MapIntegrityTests
{
    private static Assembly AssetsAssembly => typeof(MonumentAsset).Assembly;

    private static IEnumerable<string> EmbeddedAssetNames() =>
        AssetsAssembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".svg", StringComparison.Ordinal))
            .Select(n => n.Substring("RustMapsApi.Assets.Assets.".Length))
            .Select(n => n.Substring(0, n.Length - ".svg".Length));

    [Fact]
    public void EveryMappedType_ResolvesToNonEmptyStream()
    {
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            Assert.True(MonumentAssets.TryGetAsset(type, out var asset));
            using var stream = asset.OpenStream();
            Assert.True(stream.Length > 0, $"{type} -> {asset.FileName} empty");
        }
    }

    [Fact]
    public void EveryMappedAssetName_HasAnEmbeddedResource()
    {
        var embedded = EmbeddedAssetNames().ToHashSet(StringComparer.Ordinal);
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            MonumentAssets.TryGetAsset(type, out var asset);
            Assert.Contains(asset!.AssetName, embedded);
        }
    }

    [Fact]
    public void EveryEmbeddedResource_IsReferencedByTheMap()
    {
        var referenced = MonumentAssets.AvailableTypes
            .Select(t =>
            {
                MonumentAssets.TryGetAsset(t, out var a);
                return a!.AssetName;
            })
            .ToHashSet(StringComparer.Ordinal);

        foreach (var embedded in EmbeddedAssetNames())
        {
            Assert.Contains(embedded, referenced);
        }
    }

    [Fact]
    public void DistinctAssetNames_CountIs42()
    {
        var distinct = MonumentAssets.AvailableTypes
            .Select(t =>
            {
                MonumentAssets.TryGetAsset(t, out var a);
                return a!.AssetName;
            })
            .Distinct(StringComparer.Ordinal)
            .Count();

        Assert.Equal(42, distinct);
    }
}
