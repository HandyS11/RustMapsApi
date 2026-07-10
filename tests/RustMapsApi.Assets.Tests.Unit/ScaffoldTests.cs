namespace RustMapsApi.Assets.Tests.Unit;

public sealed class ScaffoldTests
{
    [Fact]
    public void AssetsAssembly_IsReferenceable()
    {
        var assembly = typeof(RustMapsApi.V4.Models.MonumentType).Assembly;
        Assert.NotNull(assembly);
    }
}
