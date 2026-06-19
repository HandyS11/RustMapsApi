namespace RustMapsApi.Results;

/// <summary>The outcome of a RustMaps API call.</summary>
/// <typeparam name="T">The payload type on success.</typeparam>
public sealed class Result<T>
{
    private Result(bool isSuccess, T? data, RustMapsError? error, int statusCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        StatusCode = statusCode;
    }

    /// <summary>Whether the call succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>The payload, populated when <see cref="IsSuccess"/> is <c>true</c>.</summary>
    public T? Data { get; }

    /// <summary>The error, populated when <see cref="IsSuccess"/> is <c>false</c>.</summary>
    public RustMapsError? Error { get; }

    /// <summary>The HTTP status code of the response.</summary>
    public int StatusCode { get; }

    /// <summary>Creates a successful result.</summary>
    /// <param name="data">The payload to carry.</param>
    /// <param name="statusCode">The HTTP status code of the response.</param>
    /// <returns>A successful <see cref="Result{T}"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1000:Do not declare static members on generic types",
        Justification = "Factory methods are the intended construction API for this result type.")]
    public static Result<T> Success(T data, int statusCode) =>
        new(true, data, null, statusCode);

    /// <summary>Creates a failed result.</summary>
    /// <param name="error">The error describing the failure.</param>
    /// <param name="statusCode">The HTTP status code of the response.</param>
    /// <returns>A failed <see cref="Result{T}"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design", "CA1000:Do not declare static members on generic types",
        Justification = "Factory methods are the intended construction API for this result type.")]
    public static Result<T> Failure(RustMapsError error, int statusCode) =>
        new(false, default, error, statusCode);
}
