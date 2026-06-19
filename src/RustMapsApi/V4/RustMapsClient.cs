using System.Net.Http;
using System.Text;
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

    /// <inheritdoc/>
    public async Task<Result<MapInfo>> GetMapBySeedAndSizeAsync(int size, int seed, bool staging, CancellationToken cancellationToken = default)
    {
        var path = $"{BasePath}/{size}/{seed}?staging={(staging ? "true" : "false")}";
        using var response = await _httpClient.GetAsync(path, cancellationToken).ConfigureAwait(false);
        return await ResultFactory.FromResponseAsync<MapInfo>(response, _jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Result<MapInfo>> GetMapByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var path = $"{BasePath}/url?url={Uri.EscapeDataString(url)}";
        using var response = await _httpClient.GetAsync(path, cancellationToken).ConfigureAwait(false);
        return await ResultFactory.FromResponseAsync<MapInfo>(response, _jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public Task<Result<MapGenerationStatus>> CreateMapAsync(MapGenerationRequest request, CancellationToken cancellationToken = default) =>
        PostJsonAsync<MapGenerationRequest, MapGenerationStatus>(BasePath, request, orgId: null, cancellationToken);

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Reliability", "CA2000:Dispose objects before losing scope",
        Justification = "Child HttpContent instances are owned and disposed by the enclosing MultipartFormDataContent.")]
    public async Task<Result<UploadedMap>> UploadMapAsync(MapUpload upload, CancellationToken cancellationToken = default)
    {
#if NET
        ArgumentNullException.ThrowIfNull(upload);
#else
        if (upload is null)
        {
            throw new ArgumentNullException(nameof(upload));
        }
#endif

        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(upload.Map);
        content.Add(fileContent, "map", upload.FileName);
        content.Add(new StringContent(upload.Staging ? "true" : "false"), "staging");
        if (upload.Note is not null)
        {
            content.Add(new StringContent(upload.Note), "note");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{BasePath}/upload") { Content = content };
        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await ResultFactory.FromResponseAsync<UploadedMap>(response, _jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<MapThumbnail>>> SearchByFilterAsync(
        string filterId, int page, SearchOptions? options = null, string? orgId = null, CancellationToken cancellationToken = default)
    {
        var path = $"{BasePath}/filter/{Uri.EscapeDataString(filterId)}{QueryStringBuilder.Build(page, options)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        if (orgId is not null)
        {
            request.Headers.Add("x-org-id", orgId);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await ResultFactory.FromPagedResponseAsync<MapThumbnail>(response, _jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Result<IReadOnlyList<MapThumbnail>>> SearchAsync(
        SearchQuery query, int page, SearchOptions? options = null, string? orgId = null, CancellationToken cancellationToken = default)
    {
        var path = $"{BasePath}/search{QueryStringBuilder.Build(page, options)}";
        using var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent(new MapSearchRequest { SearchQuery = query }),
        };
        if (orgId is not null)
        {
            request.Headers.Add("x-org-id", orgId);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await ResultFactory.FromPagedResponseAsync<MapThumbnail>(response, _jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    // Remaining members implemented in Tasks 14-15.
    // Temporary NotImplemented stubs keep the interface satisfied until then.

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

    private async Task<Result<TResponse>> PostJsonAsync<TRequest, TResponse>(
        string path, TRequest body, string? orgId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path)
        {
            Content = JsonContent(body),
        };
        if (orgId is not null)
        {
            request.Headers.Add("x-org-id", orgId);
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        return await ResultFactory.FromResponseAsync<TResponse>(response, _jsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    private StringContent JsonContent<TRequest>(TRequest body)
    {
        var json = JsonSerializer.Serialize(body, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}
