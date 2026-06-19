using global::Microsoft.Extensions.DependencyInjection;
using global::Microsoft.Extensions.Options;
using RustMapsApi;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.Unit.V4.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddRustMapsClientV4_RegistersTypedClient()
    {
        var services = new ServiceCollection();

        services.AddRustMapsClientV4(options => options.ApiKey = "key-123");
        using var provider = services.BuildServiceProvider();

        var client = provider.GetService<IRustMapsClient>();
        Assert.NotNull(client);
    }

    [Fact]
    public void AddRustMapsClientV4_EmptyApiKey_FailsValidationOnStart()
    {
        var services = new ServiceCollection();
        services.AddRustMapsClientV4(options => options.ApiKey = "");
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<RustMapsClientOptions>>();
        Assert.Throws<OptionsValidationException>(() => _ = options.Value);
    }
}
