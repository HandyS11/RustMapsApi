namespace RustMapsApi.V4.Models;

/// <summary>A map uploaded to RustMaps.</summary>
public sealed record UploadedMap
{
    /// <summary>The uploaded-map identifier.</summary>
    public string? Id { get; init; }

    /// <summary>The map thumbnail URL.</summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>When the map was uploaded (UTC).</summary>
    public DateTimeOffset UploadedUtc { get; init; }

    /// <summary>The current lifecycle state.</summary>
    public MapState State { get; init; }

    /// <summary>The display name.</summary>
    public string? DisplayName { get; init; }

    /// <summary>The URL slug.</summary>
    public string? Slug { get; init; }

    /// <summary>The purchase URL, when applicable.</summary>
    public string? PurchaseUrl { get; init; }

    /// <summary>The map seed, when known.</summary>
    public int? Seed { get; init; }

    /// <summary>The map save-file download URL, when available.</summary>
    public string? DownloadUrl { get; init; }

    /// <summary>An optional note attached to the upload.</summary>
    public string? Note { get; init; }

    /// <summary>The estimated date the map will be deleted, when applicable.</summary>
    public DateTimeOffset? EstimatedDeletionDate { get; init; }
}
