using global::Microsoft.Extensions.Configuration;
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

    [Fact]
    public void AddRustMapsClientV4_WithIConfiguration_RegistersTypedClient()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "ApiKey", "cfg-key-456" } })
            .Build();

        var services = new ServiceCollection();
        services.AddRustMapsClientV4(configuration);
        using var provider = services.BuildServiceProvider();

        var client = provider.GetService<IRustMapsClient>();
        Assert.NotNull(client);
    }

    [Fact]
    public void AddRustMapsClientV4_ResolvingClient_ExecutesHttpClientFactory()
    {
        var services = new ServiceCollection();
        services.AddRustMapsClientV4(options =>
        {
            options.ApiKey = "factory-key-789";
            options.BaseAddress = new Uri("https://api.rustmaps.com");
            options.Timeout = TimeSpan.FromSeconds(15);
        });
        using var provider = services.BuildServiceProvider();

        var client = provider.GetRequiredService<IRustMapsClient>();
        Assert.NotNull(client);
    }
}
