namespace RustMapsApi.V4.Models;

/// <summary>Island-generation settings.</summary>
public sealed record IslandConfiguration
{
    /// <summary>Whether island generation is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The island-generation intensity.</summary>
    public int Intensity { get; init; }
}
