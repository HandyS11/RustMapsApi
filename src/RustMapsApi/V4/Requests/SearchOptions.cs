namespace RustMapsApi.V4.Requests;

/// <summary>Optional query parameters for map search and filter calls.</summary>
public sealed record SearchOptions
{
    /// <summary>Restrict to staging maps.</summary>
    public bool? Staging { get; init; }

    /// <summary>Include custom maps.</summary>
    public bool? CustomMaps { get; init; }

    /// <summary>The sort order.</summary>
    public string? SortBy { get; init; }

    /// <summary>Include all save protocols.</summary>
    public bool? IncludeAllProtocols { get; init; }

    /// <summary>Exclude previously visited maps.</summary>
    public bool? IgnoreVisitedMaps { get; init; }
}
