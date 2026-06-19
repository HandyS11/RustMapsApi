namespace RustMapsApi.V4.Models;

/// <summary>Full information about a generated map.</summary>
public sealed record MapInfo
{
    /// <summary>The map identifier.</summary>
    public string? Id { get; init; }

    /// <summary>The map type.</summary>
    public string? Type { get; init; }

    /// <summary>The map seed.</summary>
    public int Seed { get; init; }

    /// <summary>The map size.</summary>
    public int Size { get; init; }

    /// <summary>The Rust save version the map was generated for.</summary>
    public int SaveVersion { get; init; }

    /// <summary>The RustMaps URL for the map.</summary>
    public string? Url { get; init; }

    /// <summary>The raw map image URL.</summary>
    public string? RawImageUrl { get; init; }

    /// <summary>The rendered map image URL.</summary>
    public string? ImageUrl { get; init; }

    /// <summary>The map icon image URL.</summary>
    public string? ImageIconUrl { get; init; }

    /// <summary>The map thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>Whether the map was generated on the staging branch.</summary>
    public bool IsStaging { get; init; }

    /// <summary>Whether the map is a custom map.</summary>
    public bool IsCustomMap { get; init; }

    /// <summary>Whether the map save file can be downloaded.</summary>
    public bool CanDownload { get; init; }

    /// <summary>The map save-file download URL, when available.</summary>
    public string? DownloadUrl { get; init; }

    /// <summary>The total number of monuments on the map.</summary>
    public int TotalMonuments { get; init; }

    /// <summary>The monuments placed on the map.</summary>
    public IReadOnlyList<Monument>? Monuments { get; init; }

    /// <summary>The percentage of the map that is land.</summary>
    public int LandPercentageOfMap { get; init; }

    /// <summary>The per-biome percentage breakdown.</summary>
    public Biomes? BiomePercentages { get; init; }

    /// <summary>The number of islands.</summary>
    public int Islands { get; init; }

    /// <summary>The number of mountains.</summary>
    public int Mountains { get; init; }

    /// <summary>The number of ice lakes.</summary>
    public int IceLakes { get; init; }

    /// <summary>The number of rivers.</summary>
    public int Rivers { get; init; }

    /// <summary>The number of lakes.</summary>
    public int Lakes { get; init; }

    /// <summary>The number of canyons.</summary>
    public int Canyons { get; init; }

    /// <summary>The number of oases.</summary>
    public int Oases { get; init; }

    /// <summary>The number of buildable rocks.</summary>
    public int BuildableRocks { get; init; }

    /// <summary>The estimated date the map will be deleted, when applicable.</summary>
    public DateTimeOffset? EstimatedDeletionDate { get; init; }
}
