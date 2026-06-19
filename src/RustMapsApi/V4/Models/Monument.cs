namespace RustMapsApi.V4.Models;

/// <summary>A monument placed on a map.</summary>
public sealed record Monument
{
    /// <summary>The monument type.</summary>
    public MonumentType Type { get; init; }

    /// <summary>The monument position.</summary>
    public Coordinates? Coordinates { get; init; }

    /// <summary>An optional display-name override.</summary>
    public string? NameOverride { get; init; }
}
