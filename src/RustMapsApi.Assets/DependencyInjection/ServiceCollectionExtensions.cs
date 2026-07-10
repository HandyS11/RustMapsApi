using RustMapsApi.V4.Assets;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registration helpers for RustMaps monument icon assets.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IMonumentAssetSource"/> as a singleton.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddRustMapsAssets(this IServiceCollection services)
    {
        services.AddSingleton<IMonumentAssetSource, MonumentAssetSource>();
        return services;
    }
}
