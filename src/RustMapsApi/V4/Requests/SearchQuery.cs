namespace RustMapsApi.V4.Requests;

/// <summary>A structured map-search query.</summary>
public sealed record SearchQuery
{
    /// <summary>The map-size range.</summary>
    public MinMaxFilter? Size { get; init; }

    /// <summary>The per-biome share filters.</summary>
    public IReadOnlyList<BiomeFilter>? Biomes { get; init; }

    /// <summary>The total-monument count range.</summary>
    public MinMaxFilter? Monuments { get; init; }

    /// <summary>The large-monument filters.</summary>
    public IReadOnlyList<MonumentFilter>? LargeMonuments { get; init; }

    /// <summary>The gas-station count range.</summary>
    public MinMaxFilter? GasStations { get; init; }

    /// <summary>The supermarket count range.</summary>
    public MinMaxFilter? Supermarkets { get; init; }

    /// <summary>The warehouse count range.</summary>
    public MinMaxFilter? Warehouses { get; init; }

    /// <summary>The lighthouse count range.</summary>
    public MinMaxFilter? Lighthouses { get; init; }

    /// <summary>The island count range.</summary>
    public MinMaxFilter? Islands { get; init; }

    /// <summary>The land-percentage range.</summary>
    public MinMaxFilter? LandPercentageOfMap { get; init; }

    /// <summary>The cave count range.</summary>
    public MinMaxFilter? Caves { get; init; }

    /// <summary>The swamp count range.</summary>
    public MinMaxFilter? Swamps { get; init; }

    /// <summary>The mountain count range.</summary>
    public MinMaxFilter? Mountains { get; init; }

    /// <summary>The iceberg count range.</summary>
    public MinMaxFilter? Icebergs { get; init; }

    /// <summary>The ice-lake count range.</summary>
    public MinMaxFilter? IceLakes { get; init; }

    /// <summary>The river count range.</summary>
    public MinMaxFilter? Rivers { get; init; }

    /// <summary>The water-well count range.</summary>
    public MinMaxFilter? WaterWells { get; init; }

    /// <summary>The lake count range.</summary>
    public MinMaxFilter? Lakes { get; init; }

    /// <summary>The canyon count range.</summary>
    public MinMaxFilter? Canyons { get; init; }

    /// <summary>The oasis count range.</summary>
    public MinMaxFilter? Oases { get; init; }

    /// <summary>The buildable-rock count range.</summary>
    public MinMaxFilter? BuildableRocks { get; init; }
}
