using System.Net;
using System.Net.Http;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientCoreTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task GetMapByIdAsync_SendsGetToExpectedRoute()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK,
            "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{\"id\":\"abc\"}}");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("abc");

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("/v4/maps/abc", handler.LastRequest.RequestUri!.AbsolutePath);
    }
}
