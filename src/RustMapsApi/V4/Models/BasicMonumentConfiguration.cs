namespace RustMapsApi.V4.Models;

/// <summary>Base configuration for a monument on a custom map.</summary>
public record BasicMonumentConfiguration
{
    /// <summary>The monument type.</summary>
    public MonumentType Type { get; init; }

    /// <summary>Whether the monument is blocked from spawning.</summary>
    public bool Blocked { get; init; }

    /// <summary>Whether the monument may set surrounding biomes.</summary>
    public bool AllowedToSetBiomes { get; init; }

    /// <summary>The per-biome preferences for the monument.</summary>
    public IReadOnlyList<MonumentBiomePreference>? BiomePreferences { get; init; }
}
