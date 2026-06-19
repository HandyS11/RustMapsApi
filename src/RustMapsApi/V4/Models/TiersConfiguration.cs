namespace RustMapsApi.V4.Models;

/// <summary>Loot-tier distribution settings.</summary>
public sealed record TiersConfiguration
{
    /// <summary>Whether tier modification is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The tier 0 percentage.</summary>
    public float? Tier0Percentage { get; init; }

    /// <summary>The tier 1 percentage.</summary>
    public float? Tier1Percentage { get; init; }

    /// <summary>The tier 2 percentage.</summary>
    public float? Tier2Percentage { get; init; }
}
