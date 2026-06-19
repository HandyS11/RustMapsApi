using System.Net;
using System.Net.Http;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientDefaultConfigTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task GetDefaultCustomConfigAsync_BuildsCustomRoute()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":{}}");
        var client = CreateClient(handler);

        var result = await client.GetDefaultCustomConfigAsync();

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(HttpMethod.Get, handler.LastRequest!.Method);
        Assert.Equal("/v4/maps/custom", handler.LastRequest.RequestUri!.AbsolutePath);
    }
}
