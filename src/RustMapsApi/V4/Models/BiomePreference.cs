namespace RustMapsApi.V4.Models;

/// <summary>A preferred biome for a monument.</summary>
public sealed record BiomePreference
{
    /// <summary>Whether the preference is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The preferred biome.</summary>
    public BiomeType Biome { get; init; }
}
