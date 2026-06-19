namespace RustMapsApi.V4.Models;

/// <summary>Generation settings for underwater labs, lakes, oases, or canyons.</summary>
public sealed record LabConfiguration
{
    /// <summary>The minimum number to generate.</summary>
    public int MinAmount { get; init; }

    /// <summary>The maximum number to generate.</summary>
    public int MaxAmount { get; init; }

    /// <summary>Whether generation is blocked.</summary>
    public bool Blocked { get; init; }

    /// <summary>Whether the feature should be generated.</summary>
    public SelectionStatus? Generate { get; init; }
}
