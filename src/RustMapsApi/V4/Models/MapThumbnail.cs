namespace RustMapsApi.V4.Models;

/// <summary>A lightweight map reference returned by search.</summary>
public sealed record MapThumbnail
{
    /// <summary>The map identifier.</summary>
    public string? MapId { get; init; }

    /// <summary>The map seed.</summary>
    public int Seed { get; init; }

    /// <summary>The map size.</summary>
    public int Size { get; init; }

    /// <summary>The RustMaps URL for the map.</summary>
    public string? Url { get; init; }
}
