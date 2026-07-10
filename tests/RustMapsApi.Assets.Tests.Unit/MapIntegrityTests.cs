using System.Reflection;
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

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

    [Fact]
    public void EveryMonumentType_IsMappedOrKnownAssetless()
    {
        // The MonumentType members with no published CDN icon: the sentinels and pure-terrain features.
        // If MonumentType gains a member, it must be added to MonumentAssetMap or to this set — otherwise
        // this test fails, flagging the unhandled value at build time.
        var knownAssetless = new HashSet<MonumentType>
        {
            MonumentType.Unknown,
            MonumentType.NotImplemented,
            MonumentType.CustomMonument,
            MonumentType.Mountain1, MonumentType.Mountain2, MonumentType.Mountain3,
            MonumentType.Mountain4, MonumentType.Mountain5,
            MonumentType.IceLake1, MonumentType.IceLake2, MonumentType.IceLake3, MonumentType.IceLake4,
            MonumentType.LargeGodRock, MonumentType.MediumGodRock, MonumentType.TinyGodRock,
            MonumentType.ThreeWallRock, MonumentType.AnvilRock,
            MonumentType.LakeA, MonumentType.LakeB, MonumentType.LakeC,
            MonumentType.CanyonA, MonumentType.CanyonB, MonumentType.CanyonC,
            MonumentType.OasisA, MonumentType.OasisB, MonumentType.OasisC,
        };

        foreach (var type in Enum.GetValues<MonumentType>())
        {
            var handled = MonumentAssets.HasAsset(type) || knownAssetless.Contains(type);
            Assert.True(
                handled,
                $"MonumentType.{type} is neither mapped in MonumentAssetMap nor listed as known-assetless. " +
                "Map it to an icon, or add it to the known-assetless set.");
        }
    }
}
