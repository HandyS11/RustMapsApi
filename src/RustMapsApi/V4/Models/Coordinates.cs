namespace RustMapsApi.V4.Models;

/// <summary>A position on the map grid.</summary>
/// <param name="X">The X coordinate.</param>
/// <param name="Y">The Y coordinate.</param>
public sealed record Coordinates(int X, int Y);
