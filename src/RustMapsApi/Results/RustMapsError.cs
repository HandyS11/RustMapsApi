namespace RustMapsApi.Results;

/// <summary>Describes why a RustMaps API call failed.</summary>
/// <param name="Kind">The category of failure.</param>
/// <param name="Message">A human-readable message, when the API supplied one.</param>
/// <param name="RawBody">The raw response body, for diagnostics.</param>
/// <param name="RetryAfter">The retry delay parsed from a 429 response, when present.</param>
public sealed record RustMapsError(
    RustMapsErrorKind Kind,
    string? Message,
    string? RawBody,
    TimeSpan? RetryAfter);
