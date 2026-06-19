namespace RustMapsApi.V4.Models;

/// <summary>Biome distribution settings.</summary>
public sealed record BiomesConfiguration
{
    /// <summary>Whether biome modification is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The arid (desert) percentage.</summary>
    public float? AridPercentage { get; init; }

    /// <summary>The temperate (forest) percentage.</summary>
    public float? TemperatePercentage { get; init; }

    /// <summary>The tundra percentage.</summary>
    public float? TundraPercentage { get; init; }

    /// <summary>The arctic (snow) percentage.</summary>
    public float? ArcticPercentage { get; init; }

    /// <summary>The jungle percentage.</summary>
    public float? JunglePercentage { get; init; }
}
