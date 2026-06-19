using RustMapsApi.Results;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.V4;

/// <summary>A strongly-typed client for the RustMaps API v4.</summary>
public interface IRustMapsClient
{
    /// <summary>Searches maps using a saved filter.</summary>
    /// <param name="filterId">The saved filter identifier.</param>
    /// <param name="page">The zero-based page index.</param>
    /// <param name="options">Optional query parameters.</param>
    /// <param name="orgId">The organisation to scope to, if any.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The matching map thumbnails.</returns>
    Task<Result<IReadOnlyList<MapThumbnail>>> SearchByFilterAsync(
        string filterId, int page, SearchOptions? options = null, string? orgId = null, CancellationToken cancellationToken = default);

    /// <summary>Searches maps using a structured query.</summary>
    /// <param name="query">The search query.</param>
    /// <param name="page">The zero-based page index.</param>
    /// <param name="options">Optional query parameters.</param>
    /// <param name="orgId">The organisation to scope to, if any.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The matching map thumbnails.</returns>
    Task<Result<IReadOnlyList<MapThumbnail>>> SearchAsync(
        SearchQuery query, int page, SearchOptions? options = null, string? orgId = null, CancellationToken cancellationToken = default);

    /// <summary>Gets a map by its identifier.</summary>
    /// <param name="mapId">The map identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The map information.</returns>
    Task<Result<MapInfo>> GetMapByIdAsync(string mapId, CancellationToken cancellationToken = default);

    /// <summary>Gets a procedural map by its size and seed.</summary>
    /// <param name="size">The map size.</param>
    /// <param name="seed">The map seed.</param>
    /// <param name="staging">Whether to query the staging branch.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The map information.</returns>
    Task<Result<MapInfo>> GetMapBySeedAndSizeAsync(int size, int seed, bool staging, CancellationToken cancellationToken = default);

    /// <summary>Gets a map by its RustMaps URL.</summary>
    /// <param name="url">The RustMaps map URL.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The map information.</returns>
    Task<Result<MapInfo>> GetMapByUrlAsync(string url, CancellationToken cancellationToken = default);

    /// <summary>Requests generation of a standard map.</summary>
    /// <param name="request">The generation parameters.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The generation status.</returns>
    Task<Result<MapGenerationStatus>> CreateMapAsync(MapGenerationRequest request, CancellationToken cancellationToken = default);

    /// <summary>Uploads a pre-generated map save file.</summary>
    /// <param name="upload">The upload payload.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The uploaded map.</returns>
    Task<Result<UploadedMap>> UploadMapAsync(MapUpload upload, CancellationToken cancellationToken = default);

    /// <summary>Gets the current map-generation limits.</summary>
    /// <param name="orgId">The organisation to scope to, if any.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The generation limits.</returns>
    Task<Result<MapGenLimits>> GetLimitsAsync(string? orgId = null, CancellationToken cancellationToken = default);

    /// <summary>Gets the caller's saved custom-map configs.</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The saved, named configs.</returns>
    Task<Result<IReadOnlyList<MapSettings>>> GetSavedConfigsAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the custom-map settings used to generate a map.</summary>
    /// <param name="mapId">The map identifier.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The settings tree.</returns>
    Task<Result<CustomMapSettings>> GetMapSettingsAsync(string mapId, CancellationToken cancellationToken = default);

    /// <summary>Gets the default custom-map configuration.</summary>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The default settings.</returns>
    Task<Result<CustomMapSettings>> GetDefaultCustomConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>Requests generation of a custom map from explicit settings.</summary>
    /// <param name="request">The custom-map request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The generation status.</returns>
    Task<Result<MapGenerationStatus>> CreateCustomMapAsync(CreateCustomMapRequest request, CancellationToken cancellationToken = default);

    /// <summary>Requests generation of a custom map from a saved config.</summary>
    /// <param name="request">The custom-map-from-config request.</param>
    /// <param name="cancellationToken">A token to cancel the request.</param>
    /// <returns>The generation status.</returns>
    Task<Result<MapGenerationStatus>> CreateCustomMapFromConfigAsync(CreateCustomMapFromConfigRequest request, CancellationToken cancellationToken = default);
}
