namespace RustMapsApi.Results;

/// <summary>Status reported by the RustMaps response envelope.</summary>
public enum ResponseStatus
{
    /// <summary>The request succeeded.</summary>
    Success = 0,

    /// <summary>The request failed.</summary>
    Failed = 1,
}
