using System.Diagnostics.CodeAnalysis;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>An injectable source of RustMaps monument icon assets.</summary>
public interface IMonumentAssetSource
{
    /// <summary>All monument types that have an icon asset.</summary>
    IReadOnlyCollection<MonumentType> AvailableTypes { get; }

    /// <summary>Attempts to resolve the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <param name="asset">The resolved asset when this returns <c>true</c>.</param>
    /// <returns><c>true</c> if the type has an icon; otherwise <c>false</c>.</returns>
    bool TryGetAsset(MonumentType type, [MaybeNullWhen(false)] out MonumentAsset asset);

    /// <summary>Resolves the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <returns>The resolved asset.</returns>
    MonumentAsset GetAsset(MonumentType type);

    /// <summary>Indicates whether a monument type has an icon asset.</summary>
    /// <param name="type">The monument type.</param>
    /// <returns><c>true</c> if an icon exists for the type.</returns>
    bool HasAsset(MonumentType type);
}
