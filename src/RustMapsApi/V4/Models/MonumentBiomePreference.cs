namespace RustMapsApi.V4.Models;

/// <summary>A biome preference for a specific monument.</summary>
public sealed record MonumentBiomePreference
{
    /// <summary>The biome the preference applies to.</summary>
    public BiomeType BiomeType { get; init; }

    /// <summary>Whether the biome is wanted.</summary>
    public SelectionStatus Selection { get; init; }
}
