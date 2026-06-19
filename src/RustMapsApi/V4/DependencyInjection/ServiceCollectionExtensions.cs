using global::Microsoft.Extensions.Configuration;
using RustMapsApi;
using RustMapsApi.V4;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registration helpers for the RustMaps API v4 client.</summary>
public static class ServiceCollectionExtensions
{
    private const string ApiKeyHeader = "X-API-Key";

    /// <summary>Registers <see cref="IRustMapsClient"/> using a configuration delegate.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Configures the client options.</param>
    /// <returns>The HTTP client builder for further configuration.</returns>
    public static IHttpClientBuilder AddRustMapsClientV4(
        this IServiceCollection services, Action<RustMapsClientOptions> configure)
    {
        services.AddOptions<RustMapsClientOptions>()
            .Configure(configure)
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "RustMaps ApiKey must be provided.")
            .ValidateOnStart();

        return Register(services);
    }

    /// <summary>Registers <see cref="IRustMapsClient"/> binding from configuration.</summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section to bind options from.</param>
    /// <returns>The HTTP client builder for further configuration.</returns>
    public static IHttpClientBuilder AddRustMapsClientV4(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RustMapsClientOptions>()
            .Bind(configuration)
            .Validate(o => !string.IsNullOrWhiteSpace(o.ApiKey), "RustMaps ApiKey must be provided.")
            .ValidateOnStart();

        return Register(services);
    }

    private static IHttpClientBuilder Register(IServiceCollection services)
    {
        return services.AddHttpClient<IRustMapsClient, RustMapsClient>((provider, http) =>
        {
            var options = provider.GetRequiredService<
                global::Microsoft.Extensions.Options.IOptions<RustMapsClientOptions>>().Value;
            http.BaseAddress = options.BaseAddress;
            http.Timeout = options.Timeout;
            http.DefaultRequestHeaders.Add(ApiKeyHeader, options.ApiKey);
        });
    }
}
