namespace RustMapsApi.Results;

/// <summary>Categorises a failed RustMaps API call.</summary>
public enum RustMapsErrorKind
{
    /// <summary>The requested resource was not found (HTTP 404).</summary>
    NotFound = 0,

    /// <summary>The API key is missing or invalid (HTTP 401).</summary>
    Unauthorized = 1,

    /// <summary>The key lacks access to the resource (HTTP 403).</summary>
    Forbidden = 2,

    /// <summary>The rate limit was exceeded (HTTP 429).</summary>
    RateLimited = 3,

    /// <summary>The map exists but has not finished generating (HTTP 409).</summary>
    Queued = 4,

    /// <summary>The request body or parameters were invalid (HTTP 400).</summary>
    Validation = 5,

    /// <summary>A network or transport failure occurred.</summary>
    Transport = 6,

    /// <summary>An unrecognised failure occurred.</summary>
    Unknown = 7,
}
