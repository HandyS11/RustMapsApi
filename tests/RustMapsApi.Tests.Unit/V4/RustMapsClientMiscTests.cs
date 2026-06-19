using System.Net;
using System.Net.Http;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientMiscTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task GetLimitsAsync_SendsOrgHeaderWhenProvided()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK,
            "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{\"concurrent\":{\"current\":0,\"allowed\":1}}}");
        var client = CreateClient(handler);

        var result = await client.GetLimitsAsync(orgId: "org-7");

        Assert.True(result.IsSuccess);
        Assert.Equal("/v4/maps/limits", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.True(handler.LastRequest.Headers.Contains("x-org-id"));
    }

    [Fact]
    public async Task GetSavedConfigsAsync_ReturnsList()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":[]}");
        var client = CreateClient(handler);

        var result = await client.GetSavedConfigsAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal("/v4/maps/custom/saved-configs", handler.LastRequest!.RequestUri!.AbsolutePath);
    }
}
