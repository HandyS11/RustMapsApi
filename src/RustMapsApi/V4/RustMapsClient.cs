using System.Net.Http;
using System.Text.Json;
using RustMapsApi.Http;
using RustMapsApi.Results;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;
using RustMapsApi.V4.Serialization;

namespace RustMapsApi.V4;

/// <inheritdoc cref="IRustMapsClient"/>
public sealed class RustMapsClient : IRustMapsClient
{
    private const string BasePath = "v4/maps";

    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>Creates a client over the supplied <see cref="HttpClient"/>.</summary>
    /// <param name="httpClient">The configured HTTP client (base address and auth header set by the caller).</param>
    public RustMapsClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = RustMapsJsonOptions.Create(RustMapsJsonContextV4.Default);
    }

    /// <inheritdoc/>
    public async Task<Result<MapInfo>> GetMapByIdAsync(string mapId, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient
            .GetAsync($"{BasePath}/{Uri.EscapeDataString(mapId)}", cancellationToken)
            .ConfigureAwait(false);
        return await ResultFactory.FromResponseAsync<MapInfo>(response, _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    // Remaining members implemented in Tasks 12-15.
    // Temporary NotImplemented stubs keep the interface satisfied until then.

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<MapThumbnail>>> SearchByFilterAsync(
        string filterId, int page, SearchOptions? options = null, string? orgId = null, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<MapThumbnail>>> SearchAsync(
        SearchQuery query, int page, SearchOptions? options = null, string? orgId = null, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapInfo>> GetMapBySeedAndSizeAsync(int size, int seed, bool staging, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapInfo>> GetMapByUrlAsync(string url, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapGenerationStatus>> CreateMapAsync(MapGenerationRequest request, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<UploadedMap>> UploadMapAsync(MapUpload upload, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapGenLimits>> GetLimitsAsync(string? orgId = null, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<IReadOnlyList<CustomMapSettings>>> GetSavedConfigsAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapSettings>> GetMapSettingsAsync(string mapId, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<CustomMapSettings>> GetDefaultCustomConfigAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapGenerationStatus>> CreateCustomMapAsync(CreateCustomMapRequest request, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<Result<MapGenerationStatus>> CreateCustomMapFromConfigAsync(CreateCustomMapFromConfigRequest request, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
}
