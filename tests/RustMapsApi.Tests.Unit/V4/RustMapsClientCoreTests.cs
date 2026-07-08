using System.Net;
using System.Net.Http;
using RustMapsApi.V4;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientCoreTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public void Constructor_NullHttpClient_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new RustMapsClient(null!));

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

    [Fact]
    public async Task GetMapByIdAsync_UnknownMonumentType_DeserializesToUnknownAndKeepsRawImageUrl()
    {
        const string json = """
            {"meta":{"status":"Success","statusCode":200},
            "data":{"id":"abc","rawImageUrl":"https://example/raw.png",
            "monuments":[{"type":99999}]}}
            """;
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, json);
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("abc");

        Assert.True(result.IsSuccess);
        Assert.Equal(MonumentType.Unknown, result.Data!.Monuments![0].Type);
        Assert.Equal("https://example/raw.png", result.Data.RawImageUrl);
    }
}
