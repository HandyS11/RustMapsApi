using System.Net;
using System.Net.Http;
using RustMapsApi.V4;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.V4;

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
}
