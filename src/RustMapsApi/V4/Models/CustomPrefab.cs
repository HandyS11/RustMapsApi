namespace RustMapsApi.V4.Models;

/// <summary>A custom prefab applied to a monument slot.</summary>
public sealed record CustomPrefab
{
    /// <summary>Whether the custom prefab is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The custom prefab identifier.</summary>
    public string? Id { get; init; }
}
