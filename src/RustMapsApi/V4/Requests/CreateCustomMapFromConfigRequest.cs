using System.Text.Json.Serialization;

namespace RustMapsApi.V4.Requests;

/// <summary>A request to generate a custom map from a saved config.</summary>
public sealed record CreateCustomMapFromConfigRequest
{
    /// <summary>The organisation identifier, sent as the <c>x-org-id</c> header.</summary>
    [JsonIgnore]
    public string? OrgId { get; init; }

    /// <summary>The base map-generation parameters.</summary>
    public required MapGenerationRequest MapParameters { get; init; }

    /// <summary>The name of the saved config to use.</summary>
    public required string ConfigName { get; init; }
}
