namespace RustMapsApi.V4.Requests;

using RustMapsApi.V4.Models;

/// <summary>A filter on a specific monument.</summary>
public sealed record MonumentFilter
{
    /// <summary>The monument type to filter on.</summary>
    public MonumentType Type { get; init; }

    /// <summary>Whether the monument is wanted.</summary>
    public SelectionStatus SelectionStatus { get; init; }

    /// <summary>Biomes the monument must spawn in.</summary>
    public IReadOnlyList<BiomeType>? RequiredBiomes { get; init; }

    /// <summary>Biomes the monument must not spawn in.</summary>
    public IReadOnlyList<BiomeType>? BlockedBiomes { get; init; }
}
