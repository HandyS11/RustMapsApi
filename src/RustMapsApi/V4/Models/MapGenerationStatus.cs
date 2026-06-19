namespace RustMapsApi.V4.Models;

/// <summary>The generation status of a map.</summary>
public sealed record MapGenerationStatus
{
    /// <summary>The map identifier, once assigned.</summary>
    public string? MapId { get; init; }

    /// <summary>The position in the generation queue, when queued.</summary>
    public int? QueuePosition { get; init; }

    /// <summary>The current lifecycle state.</summary>
    public MapState State { get; init; }

    /// <summary>The current generation step, when generating.</summary>
    public string? CurrentStep { get; init; }

    /// <summary>The last time the generator reported progress (UTC).</summary>
    public DateTimeOffset? LastGeneratorPingUtc { get; init; }
}
