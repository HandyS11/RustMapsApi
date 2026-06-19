using RustMapsApi.Results;

namespace RustMapsApi.Http;

/// <summary>The metadata block of a RustMaps response envelope.</summary>
internal sealed record Meta
{
    /// <summary>The status reported by the envelope.</summary>
    public ResponseStatus Status { get; init; }

    /// <summary>The HTTP status code echoed by the envelope.</summary>
    public int StatusCode { get; init; }

    /// <summary>The error messages supplied on failure, if any.</summary>
    public IReadOnlyList<string>? Errors { get; init; }
}

/// <summary>A RustMaps response envelope wrapping a single payload.</summary>
/// <typeparam name="T">The payload type.</typeparam>
internal sealed record ServiceResponse<T>
{
    /// <summary>The response metadata.</summary>
    public Meta? Meta { get; init; }

    /// <summary>The payload.</summary>
    public T? Data { get; init; }
}

/// <summary>A RustMaps response envelope wrapping a paged payload.</summary>
/// <typeparam name="T">The payload type.</typeparam>
internal sealed record PagedServiceResponse<T>
{
    /// <summary>The response metadata.</summary>
    public Meta? Meta { get; init; }

    /// <summary>The payload.</summary>
    public T? Data { get; init; }
}
