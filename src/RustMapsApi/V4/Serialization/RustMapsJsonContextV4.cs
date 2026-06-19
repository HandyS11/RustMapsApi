using System.Text.Json.Serialization;
using RustMapsApi.Http;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.V4.Serialization;

/// <summary>The source-gen serialization context for the RustMaps API v4 surface.</summary>
[JsonSerializable(typeof(ServiceResponse<MapInfo>))]
[JsonSerializable(typeof(ServiceResponse<MapGenerationStatus>))]
[JsonSerializable(typeof(ServiceResponse<MapGenLimits>))]
[JsonSerializable(typeof(ServiceResponse<MapSettings>))]
[JsonSerializable(typeof(ServiceResponse<CustomMapSettings>))]
[JsonSerializable(typeof(ServiceResponse<UploadedMap>))]
[JsonSerializable(typeof(ServiceResponse<IReadOnlyList<CustomMapSettings>>))]
[JsonSerializable(typeof(ServiceResponse<object>))]
[JsonSerializable(typeof(PagedServiceResponse<IReadOnlyList<MapThumbnail>>))]
[JsonSerializable(typeof(MapGenerationRequest))]
[JsonSerializable(typeof(SearchQuery))]
[JsonSerializable(typeof(CreateCustomMapRequest))]
[JsonSerializable(typeof(CreateCustomMapFromConfigRequest))]
internal sealed partial class RustMapsJsonContextV4 : JsonSerializerContext;
