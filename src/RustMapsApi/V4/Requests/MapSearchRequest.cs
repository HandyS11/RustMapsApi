namespace RustMapsApi.V4.Requests;

/// <summary>The body wrapper for a raw map search.</summary>
internal sealed record MapSearchRequest
{
    /// <summary>The search query.</summary>
    public SearchQuery? SearchQuery { get; init; }
}
