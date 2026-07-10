using System.Diagnostics.CodeAnalysis;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>Default <see cref="IMonumentAssetSource"/> delegating to <see cref="MonumentAssets"/>.</summary>
public sealed class MonumentAssetSource : IMonumentAssetSource
{
    /// <inheritdoc />
    public bool TryGetAsset(MonumentType type, [MaybeNullWhen(false)] out MonumentAsset asset) =>
        MonumentAssets.TryGetAsset(type, out asset);

    /// <inheritdoc />
    public MonumentAsset GetAsset(MonumentType type) => MonumentAssets.GetAsset(type);

    /// <inheritdoc />
    public bool HasAsset(MonumentType type) => MonumentAssets.HasAsset(type);

    /// <inheritdoc />
    public IReadOnlyCollection<MonumentType> AvailableTypes => MonumentAssets.AvailableTypes;
}
