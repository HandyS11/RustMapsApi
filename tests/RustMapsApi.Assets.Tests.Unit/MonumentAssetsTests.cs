using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class MonumentAssetsTests
{
    [Fact]
    public void TryGetAsset_MappedType_ReturnsTrueAndAsset()
    {
        var ok = MonumentAssets.TryGetAsset(MonumentType.NuclearMissileSilo, out var asset);
        Assert.True(ok);
        Assert.Equal("Nuclear_Missile_Silo", asset!.AssetName);
    }

    [Theory]
    [InlineData(MonumentType.CaveLargeHard, "Cave")]
    [InlineData(MonumentType.CaveSmallEasy, "Cave")]
    [InlineData(MonumentType.HarborLarge, "Harbor")]
    [InlineData(MonumentType.StablesA, "Large_Barn")]
    [InlineData(MonumentType.JungleZigguratA, "Ziggurat")]
    [InlineData(MonumentType.OilrigLarge, "Oilrig_Large")]
    [InlineData(MonumentType.OilrigSmall, "Oilrig_Small")]
    [InlineData(MonumentType.TunnelEntranceTransition, "Tunnel_Entrance_Transition")]
    [InlineData(MonumentType.ApartmentsComplex, "Apartments_Complex")]
    public void TryGetAsset_ResolvesExpectedAssetName(MonumentType type, string expected)
    {
        Assert.True(MonumentAssets.TryGetAsset(type, out var asset));
        Assert.Equal(expected, asset.AssetName);
    }

    [Theory]
    [InlineData(MonumentType.Unknown)]
    [InlineData(MonumentType.NotImplemented)]
    [InlineData(MonumentType.CustomMonument)]
    [InlineData(MonumentType.Mountain1)]
    [InlineData(MonumentType.IceLake1)]
    [InlineData(MonumentType.LakeA)]
    [InlineData(MonumentType.CanyonA)]
    [InlineData(MonumentType.OasisA)]
    [InlineData(MonumentType.LargeGodRock)]
    [InlineData(MonumentType.AnvilRock)]
    public void TryGetAsset_AssetlessType_ReturnsFalse(MonumentType type)
    {
        Assert.False(MonumentAssets.TryGetAsset(type, out _));
        Assert.False(MonumentAssets.HasAsset(type));
    }

    [Fact]
    public void GetAsset_AssetlessType_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => MonumentAssets.GetAsset(MonumentType.Mountain1));
    }

    [Fact]
    public void AvailableTypes_MatchesHasAsset()
    {
        Assert.NotEmpty(MonumentAssets.AvailableTypes);
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            Assert.True(MonumentAssets.HasAsset(type));
        }
    }
}
