namespace RustMapsApi.V4.Models;

/// <summary>The placement of an oil rig along an edge.</summary>
public sealed record OilRigPosition
{
    /// <summary>Whether explicit positioning is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>The edge the rig is aligned to.</summary>
    public MonumentAlignment Alignment { get; init; }

    /// <summary>The position along the edge.</summary>
    public float Position { get; init; }
}
