using System.Net;
using System.Net.Http;
using RustMapsApi.Results;
using RustMapsApi.V4;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientMapsTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task GetMapBySeedAndSizeAsync_BuildsRouteWithStagingQuery()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{\"id\":\"x\"}}");
        var client = CreateClient(handler);

        await client.GetMapBySeedAndSizeAsync(4500, 12345, staging: true);

        Assert.Equal("/v4/maps/4500/12345", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Contains("staging=true", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task GetMapBySeedAndSizeAsync_StagingFalse_BuildsFalseQuery()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{\"id\":\"x\"}}");
        var client = CreateClient(handler);

        await client.GetMapBySeedAndSizeAsync(4500, 12345, staging: false);

        Assert.Contains("staging=false", handler.LastRequest!.RequestUri!.Query);
        Assert.DoesNotContain("staging=true", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task GetMapByUrlAsync_PutsUrlInQuery()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{\"id\":\"x\"}}");
        var client = CreateClient(handler);

        await client.GetMapByUrlAsync("https://rustmaps.com/map/abc");

        Assert.Equal("/v4/maps/url", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Contains("url=https", Uri.UnescapeDataString(handler.LastRequest.RequestUri.Query));
    }

    [Fact]
    public async Task CreateMapAsync_Created_ReturnsStatus()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.Created,
            "{\"meta\":{\"status\":\"Success\",\"statusCode\":201},\"data\":{\"mapId\":\"x\",\"state\":\"inQueue\"}}");
        var client = CreateClient(handler);

        var result = await client.CreateMapAsync(new MapGenerationRequest
        {
            Size = 4500, Seed = 1, Staging = false
        });

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/v4/maps", handler.LastRequest.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateMapAsync_Conflict_ReturnsQueuedError()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.Conflict, "{\"meta\":{\"status\":\"Failed\",\"statusCode\":409},\"data\":{\"id\":\"x\"}}");
        var client = CreateClient(handler);

        var result = await client.CreateMapAsync(new MapGenerationRequest
        {
            Size = 4500, Seed = 1, Staging = false
        });

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Queued, result.Error!.Kind);
    }

    [Fact]
    public async Task CreateMapAsync_NullRequest_Throws()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateMapAsync(null!));
    }
}
