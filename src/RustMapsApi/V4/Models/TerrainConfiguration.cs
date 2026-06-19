namespace RustMapsApi.V4.Models;

/// <summary>Terrain-shaping settings for a custom map.</summary>
public sealed record TerrainConfiguration
{
    /// <summary>The island configuration.</summary>
    public IslandConfiguration? IslandConfig { get; init; }

    /// <summary>The mountain configuration.</summary>
    public MountainConfiguration? MountainConfig { get; init; }

    /// <summary>The loot-tier configuration.</summary>
    public TiersConfiguration? TierConfig { get; init; }

    /// <summary>The biome configuration.</summary>
    public BiomesConfiguration? BiomeConfig { get; init; }

    /// <summary>Whether the shore and bay are flattened.</summary>
    public bool? FlattenShoreAndBay { get; init; }

    /// <summary>The biome-axis orientation.</summary>
    public BiomeAngle? BiomeAxisAngle { get; init; }

    /// <summary>The loot-axis orientation.</summary>
    public LootAngle? LootAxisAngle { get; init; }
}
