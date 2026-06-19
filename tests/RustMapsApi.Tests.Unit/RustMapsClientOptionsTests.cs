using RustMapsApi;

namespace RustMapsApi.Tests.Unit;

public class RustMapsClientOptionsTests
{
    [Fact]
    public void Defaults_AreEmptyKeyAndProductionAddress()
    {
        var options = new RustMapsClientOptions();

        Assert.Equal(string.Empty, options.ApiKey);
        Assert.Equal(new Uri("https://api.rustmaps.com"), options.BaseAddress);
        Assert.Equal(TimeSpan.FromSeconds(30), options.Timeout);
    }
}
