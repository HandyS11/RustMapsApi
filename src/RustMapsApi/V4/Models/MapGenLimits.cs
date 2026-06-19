namespace RustMapsApi.V4.Models;

/// <summary>Current map-generation limits.</summary>
public sealed record MapGenLimits
{
    /// <summary>The concurrent generation limit.</summary>
    public MapGenerationStat? Concurrent { get; init; }

    /// <summary>The monthly generation limit.</summary>
    public MapGenerationStat? Monthly { get; init; }
}
