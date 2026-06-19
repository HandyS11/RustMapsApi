using System.Text.Json.Serialization;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Requests;

/// <summary>A request to generate a custom map from explicit settings.</summary>
public sealed record CreateCustomMapRequest
{
    /// <summary>The organisation identifier, sent as the <c>x-org-id</c> header.</summary>
    [JsonIgnore]
    public string? OrgId { get; init; }

    /// <summary>The base map-generation parameters.</summary>
    public required MapGenerationRequest MapParameters { get; init; }

    /// <summary>The custom-map settings tree.</summary>
    public required CustomMapSettings CustomMapSettings { get; init; }
}
