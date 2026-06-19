using RustMapsApi.Results;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Integration;

[Collection("LiveApi")]
public sealed class LiveGetEndpointsTests
{
    private readonly LiveApiFixture _fixture;

    public LiveGetEndpointsTests(LiveApiFixture fixture) => _fixture = fixture;

    [SkippableFact]
    public async Task GetLimits_Succeeds()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");

        await _fixture.ThrottleAsync();
        var result = await _fixture.Client.GetLimitsAsync();

        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [SkippableFact]
    public async Task GetSavedConfigs_Succeeds()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");

        await _fixture.ThrottleAsync();
        var result = await _fixture.Client.GetSavedConfigsAsync();

        // Account-scoped; may be empty. We assert the call succeeds, not its contents.
        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [SkippableFact]
    public async Task Search_WithSizeOnly_Succeeds()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");

        await _fixture.ThrottleAsync();
        var query = new SearchQuery
        {
            Size = new MinMaxFilter(_fixture.AnchorSize, _fixture.AnchorSize)
        };
        var result = await _fixture.Client.SearchAsync(query, page: 0);

        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [SkippableFact]
    public async Task MapChain_SeedSize_Then_ById_And_ByUrl()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");

        // Anchor: get a known public procedural map by size + seed.
        await _fixture.ThrottleAsync();
        var anchor = await _fixture.Client.GetMapBySeedAndSizeAsync(
            _fixture.AnchorSize, _fixture.AnchorSeed, staging: false);

        // If the anchor map is not generated on RustMaps' side, skip the dependent
        // assertions rather than failing — and never trigger generation (no credits).
        Skip.IfNot(anchor.IsSuccess, $"Anchor map not available: {anchor.Error?.Kind} {anchor.Error?.Message}");

        var id = anchor.Data!.Id;
        var url = anchor.Data.Url;
        Assert.False(string.IsNullOrWhiteSpace(id));
        Assert.False(string.IsNullOrWhiteSpace(url));

        // Chained: get the same map by id.
        await _fixture.ThrottleAsync();
        var byId = await _fixture.Client.GetMapByIdAsync(id!);
        Assert.True(byId.IsSuccess, byId.Error?.Message);
        Assert.Equal(id, byId.Data!.Id);

        // Chained: get the same map by url.
        await _fixture.ThrottleAsync();
        var byUrl = await _fixture.Client.GetMapByUrlAsync(url!);
        Assert.True(byUrl.IsSuccess, byUrl.Error?.Message);
    }

    [SkippableFact]
    public async Task GetDefaultCustomConfig_SucceedsOrSkipsWithoutSubscription()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");

        await _fixture.ThrottleAsync();
        var result = await _fixture.Client.GetDefaultCustomConfigAsync();

        SkipIfSubscriptionRequired(result.Error?.Kind);
        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [SkippableFact]
    public async Task GetMapSettings_SucceedsOrSkipsWithoutSubscription()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");

        // Need a real map id first.
        await _fixture.ThrottleAsync();
        var anchor = await _fixture.Client.GetMapBySeedAndSizeAsync(
            _fixture.AnchorSize, _fixture.AnchorSeed, staging: false);
        Skip.IfNot(anchor.IsSuccess, $"Anchor map not available: {anchor.Error?.Kind}");

        await _fixture.ThrottleAsync();
        var result = await _fixture.Client.GetMapSettingsAsync(anchor.Data!.Id!);

        SkipIfSubscriptionRequired(result.Error?.Kind);
        Skip.If(result.Error?.Kind == RustMapsErrorKind.NotFound, "Anchor map has no custom settings.");
        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    [SkippableFact]
    public async Task SearchByFilter_Succeeds_WhenFilterIdProvided()
    {
        Skip.IfNot(_fixture.KeyAvailable, "RUSTMAPS_API_KEY not set.");
        Skip.If(string.IsNullOrWhiteSpace(_fixture.FilterId),
            "RUSTMAPS_TEST_FILTER_ID not set (filter ids are created on the homepage).");

        await _fixture.ThrottleAsync();
        var result = await _fixture.Client.SearchByFilterAsync(_fixture.FilterId!, page: 0);

        Assert.True(result.IsSuccess, result.Error?.Message);
    }

    private static void SkipIfSubscriptionRequired(RustMapsErrorKind? kind) =>
        Skip.If(kind is RustMapsErrorKind.Unauthorized or RustMapsErrorKind.Forbidden,
            "Endpoint requires a subscription; test key is not subscribed.");
}
