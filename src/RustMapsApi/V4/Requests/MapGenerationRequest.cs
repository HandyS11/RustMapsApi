namespace RustMapsApi.V4.Requests;

/// <summary>A request to generate a standard map.</summary>
public sealed record MapGenerationRequest
{
    /// <summary>The map size.</summary>
    public int Size { get; init; }

    /// <summary>The map seed.</summary>
    public int Seed { get; init; }

    /// <summary>Whether to generate against the staging branch.</summary>
    public bool Staging { get; init; }
}
