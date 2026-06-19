using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Requests;

/// <summary>A filter on the share of a biome.</summary>
public sealed record BiomeFilter
{
    /// <summary>The biome to filter on.</summary>
    public BiomeType Type { get; init; }

    /// <summary>The percentage range to require.</summary>
    public MinMaxFilter? Settings { get; init; }
}
