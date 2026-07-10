using System.Diagnostics.CodeAnalysis;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>Resolves RustMaps monument icon assets from a <see cref="MonumentType"/>.</summary>
public static class MonumentAssets
{
    /// <summary>All monument types that have an icon asset.</summary>
    public static IReadOnlyCollection<MonumentType> AvailableTypes { get; } =
        Array.AsReadOnly(MonumentAssetMap.AssetNames.Keys.ToArray());

    /// <summary>Attempts to resolve the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <param name="asset">The resolved asset when this returns <c>true</c>.</param>
    /// <returns><c>true</c> if the type has an icon; otherwise <c>false</c>.</returns>
    public static bool TryGetAsset(MonumentType type, [MaybeNullWhen(false)] out MonumentAsset asset)
    {
        if (MonumentAssetMap.AssetNames.TryGetValue(type, out var name))
        {
            asset = new MonumentAsset(type, name);
            return true;
        }

        asset = null;
        return false;
    }

    /// <summary>Resolves the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <returns>The resolved asset.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">
    /// The type has no icon asset.</exception>
    public static MonumentAsset GetAsset(MonumentType type) =>
        TryGetAsset(type, out var asset)
            ? asset
            : throw new KeyNotFoundException($"No icon asset is mapped for MonumentType.{type}.");

    /// <summary>Indicates whether a monument type has an icon asset.</summary>
    /// <param name="type">The monument type.</param>
    /// <returns><c>true</c> if an icon exists for the type.</returns>
    public static bool HasAsset(MonumentType type) =>
        MonumentAssetMap.AssetNames.ContainsKey(type);
}
