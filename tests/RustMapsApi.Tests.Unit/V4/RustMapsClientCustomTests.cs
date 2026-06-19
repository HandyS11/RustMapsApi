using System.Net;
using System.Net.Http;
using RustMapsApi.V4;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientCustomTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task GetMapSettingsAsync_BuildsSettingsRoute()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{}}");
        var client = CreateClient(handler);

        await client.GetMapSettingsAsync("map-9");

        Assert.Equal("/v4/maps/map-9/settings", handler.LastRequest!.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task CreateCustomMapAsync_SendsOrgHeaderFromRequest()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.Created,
            "{\"meta\":{\"status\":\"Success\",\"statusCode\":201},\"data\":{\"mapId\":\"x\",\"state\":\"inQueue\"}}");
        var client = CreateClient(handler);

        var request = new CreateCustomMapRequest
        {
            OrgId = "org-3",
            MapParameters = new MapGenerationRequest
            {
                Size = 4500, Seed = 1, Staging = false
            },
            CustomMapSettings = new CustomMapSettings(),
        };

        var result = await client.CreateCustomMapAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal("/v4/maps/custom", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.True(handler.LastRequest.Headers.Contains("x-org-id"));
    }

    [Fact]
    public async Task CreateCustomMapAsync_NullRequest_Throws()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateCustomMapAsync(null!));
    }

    [Fact]
    public async Task CreateCustomMapFromConfigAsync_BuildsSavedConfigRoute()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.Created,
            "{\"meta\":{\"status\":\"Success\",\"statusCode\":201},\"data\":{\"mapId\":\"x\",\"state\":\"inQueue\"}}");
        var client = CreateClient(handler);

        var request = new CreateCustomMapFromConfigRequest
        {
            OrgId = "org-5",
            MapParameters = new MapGenerationRequest
            {
                Size = 4500, Seed = 1, Staging = false
            },
            ConfigName = "config-1",
        };

        var result = await client.CreateCustomMapFromConfigAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/v4/maps/custom/saved-config", handler.LastRequest.RequestUri!.AbsolutePath);
        Assert.True(handler.LastRequest.Headers.Contains("x-org-id"));
        Assert.NotNull(handler.LastRequest.Content);
        Assert.NotNull(handler.LastRequestBody);
        Assert.Contains("config-1", handler.LastRequestBody);
    }

    [Fact]
    public async Task CreateCustomMapFromConfigAsync_NullRequest_Throws()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.CreateCustomMapFromConfigAsync(null!));
    }
}
