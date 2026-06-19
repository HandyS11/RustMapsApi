namespace RustMapsApi.V4.Models;

/// <summary>A named, saved custom-map configuration.</summary>
public sealed record MapSettings
{
    /// <summary>The configuration identifier.</summary>
    public string? Id { get; init; }

    /// <summary>The configuration name.</summary>
    public string? Name { get; init; }

    /// <summary>The settings tree.</summary>
    public CustomMapSettings? Settings { get; init; }
}
