namespace RustMapsApi.V4.Requests;

/// <summary>A request to upload a pre-generated map save file.</summary>
public sealed record MapUpload
{
    /// <summary>The map save-file content.</summary>
    public required Stream Map { get; init; }

    /// <summary>The file name to send with the upload.</summary>
    public required string FileName { get; init; }

    /// <summary>An optional note (max 100 chars, alphanumerics and <c>_.()- </c>).</summary>
    public string? Note { get; init; }

    /// <summary>Whether the map targets the staging branch.</summary>
    public bool Staging { get; init; }
}
