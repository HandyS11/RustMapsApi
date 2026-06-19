namespace RustMapsApi.V4.Models;

/// <summary>A usage counter against an allowance.</summary>
/// <param name="Current">The amount used.</param>
/// <param name="Allowed">The maximum allowed.</param>
public sealed record MapGenerationStat(int Current, int Allowed);
