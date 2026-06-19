namespace RustMapsApi.V4.Models;

/// <summary>Mountain-generation settings.</summary>
public sealed record MountainConfiguration
{
    /// <summary>Whether the number of mountains is reduced.</summary>
    public bool ReduceMountains { get; init; }
}
