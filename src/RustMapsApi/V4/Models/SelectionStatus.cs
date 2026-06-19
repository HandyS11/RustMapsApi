namespace RustMapsApi.V4.Models;

/// <summary>Whether a feature is wanted in a custom map.</summary>
public enum SelectionStatus
{
    /// <summary>The feature is required.</summary>
    Wanted = 0,

    /// <summary>The feature is excluded.</summary>
    NotWanted = 1,

    /// <summary>No preference is expressed.</summary>
    NoPreference = 2,
}
