namespace RustMapsApi.V4.Models;

/// <summary>The lifecycle state of a map.</summary>
public enum MapState
{
    /// <summary>The map is fully generated and available.</summary>
    Active = 0,

    /// <summary>The map is queued for generation.</summary>
    InQueue = 1,

    /// <summary>The map is being generated.</summary>
    Generating = 2,

    /// <summary>The map is being processed after generation.</summary>
    Processing = 3,

    /// <summary>The map is being uploaded.</summary>
    Uploading = 4,
}
