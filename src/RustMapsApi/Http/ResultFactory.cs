using System.Net.Http;
using System.Text.Json;
using RustMapsApi.Results;

namespace RustMapsApi.Http;

/// <summary>Maps HTTP responses (and the RustMaps envelope) into <see cref="Result{T}"/>.</summary>
internal static class ResultFactory
{
    /// <summary>Maps a single-payload response into a <see cref="Result{T}"/>.</summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="response">The HTTP response to read.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <param name="cancellationToken">A token to cancel the read.</param>
    /// <returns>A success or failure result.</returns>
    public static async Task<Result<T>> FromResponseAsync<T>(
        HttpResponseMessage response,
        JsonSerializerOptions options,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
#if NET
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif

        if (response.IsSuccessStatusCode)
        {
            var envelope = JsonSerializer.Deserialize<ServiceResponse<T>>(body, options);
            if (envelope is { Data: { } data })
            {
                return Result<T>.Success(data, statusCode);
            }

            return Result<T>.Failure(
                new RustMapsError(RustMapsErrorKind.Unknown, "Response contained no data.", body, null),
                statusCode);
        }

        var kind = MapStatusCode(statusCode);
        var message = TryReadErrors(body, options);
        var retryAfter = response.Headers.RetryAfter?.Delta;
        return Result<T>.Failure(new RustMapsError(kind, message, body, retryAfter), statusCode);
    }

    /// <summary>Maps a paged-payload response into a <see cref="Result{T}"/> of a read-only list.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="response">The HTTP response to read.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <param name="cancellationToken">A token to cancel the read.</param>
    /// <returns>A success or failure result.</returns>
    public static async Task<Result<IReadOnlyList<T>>> FromPagedResponseAsync<T>(
        HttpResponseMessage response,
        JsonSerializerOptions options,
        CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
#if NET
        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
        cancellationToken.ThrowIfCancellationRequested();
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
        if (response.IsSuccessStatusCode)
        {
            var envelope = JsonSerializer.Deserialize<PagedServiceResponse<IReadOnlyList<T>>>(body, options);
            var data = envelope?.Data ?? Array.Empty<T>();
            return Result<IReadOnlyList<T>>.Success(data, statusCode);
        }

        var kind = MapStatusCode(statusCode);
        var message = TryReadErrors(body, options);
        var retryAfter = response.Headers.RetryAfter?.Delta;
        return Result<IReadOnlyList<T>>.Failure(new RustMapsError(kind, message, body, retryAfter), statusCode);
    }

    /// <summary>Maps an HTTP status code to the matching <see cref="RustMapsErrorKind"/>.</summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>The categorised error kind.</returns>
    public static RustMapsErrorKind MapStatusCode(int statusCode) => statusCode switch
    {
        400 => RustMapsErrorKind.Validation,
        401 => RustMapsErrorKind.Unauthorized,
        403 => RustMapsErrorKind.Forbidden,
        404 => RustMapsErrorKind.NotFound,
        409 => RustMapsErrorKind.Queued,
        429 => RustMapsErrorKind.RateLimited,
        _ => RustMapsErrorKind.Unknown,
    };

    private static string? TryReadErrors(string body, JsonSerializerOptions options)
    {
        try
        {
            var envelope = JsonSerializer.Deserialize<ServiceResponse<object>>(body, options);
            var errors = envelope?.Meta?.Errors;
            return errors is { Count: > 0 } ? string.Join("; ", errors) : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
