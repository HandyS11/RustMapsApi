using System.Reflection;
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class MonumentAssetTests
{
    /// <summary>Constructs a <see cref="MonumentAsset"/> via the internal ctor for a known-embedded asset.</summary>
    private static MonumentAsset Cave() =>
        (MonumentAsset)Activator.CreateInstance(
            typeof(MonumentAsset),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [MonumentType.CaveLargeHard, "Cave"],
            culture: null)!;

    [Fact]
    public void Metadata_IsDerivedFromAssetName()
    {
        var asset = Cave();
        Assert.Equal(MonumentType.CaveLargeHard, asset.MonumentType);
        Assert.Equal("Cave", asset.AssetName);
        Assert.Equal("Cave.svg", asset.FileName);
        Assert.Equal("image/svg+xml", asset.MediaType);
        Assert.Equal("https://content.rustmaps.com/assets/Cave.svg", asset.SourceUri.ToString());
    }

    [Fact]
    public void OpenStream_ReturnsNonEmptyStream()
    {
        using var stream = Cave().OpenStream();
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void GetSvg_ReturnsSvgMarkup()
    {
        var svg = Cave().GetSvg();
        Assert.False(string.IsNullOrWhiteSpace(svg));
        Assert.Contains("<svg", svg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetBytes_MatchesStreamLength()
    {
        var asset = Cave();
        using var stream = asset.OpenStream();
        Assert.Equal(stream.Length, asset.GetBytes().Length);
    }

    [Fact]
    public void OpenStream_MissingEmbeddedResource_Throws()
    {
        var asset = (MonumentAsset)Activator.CreateInstance(
            typeof(MonumentAsset),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: [MonumentType.Unknown, "NoSuchAsset"],
            culture: null)!;

        var ex = Assert.Throws<InvalidOperationException>(() => asset.OpenStream());
        Assert.Contains("NoSuchAsset.svg", ex.Message, StringComparison.Ordinal);
    }
}
