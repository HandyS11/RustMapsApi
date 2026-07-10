using System.Linq;
using System.Reflection;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class EmbeddedResourceTests
{
    private static Assembly AssetsAssembly => typeof(RustMapsApi.V4.Assets.MonumentAsset).Assembly;

    [Fact]
    public void EmbeddedSvgResources_CountIs42()
    {
        var svgResources = AssetsAssembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".svg", System.StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(42, svgResources.Length);
    }

    [Fact]
    public void EmbeddedSvgResources_AreAllNonEmpty()
    {
        var svgResources = AssetsAssembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".svg", System.StringComparison.Ordinal));

        foreach (var name in svgResources)
        {
            using var stream = AssetsAssembly.GetManifestResourceStream(name);
            Assert.NotNull(stream);
            Assert.True(stream!.Length > 0, $"{name} is empty");
        }
    }
}
