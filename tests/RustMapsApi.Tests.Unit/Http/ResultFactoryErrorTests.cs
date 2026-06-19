using System.Net;
using System.Net.Http;
using RustMapsApi.Results;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.Unit.Http;

public class ResultFactoryErrorTests
{
    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Theory]
    [InlineData(HttpStatusCode.BadRequest, RustMapsErrorKind.Validation)]
    [InlineData(HttpStatusCode.Unauthorized, RustMapsErrorKind.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden, RustMapsErrorKind.Forbidden)]
    [InlineData(HttpStatusCode.NotFound, RustMapsErrorKind.NotFound)]
    [InlineData(HttpStatusCode.Conflict, RustMapsErrorKind.Queued)]
    [InlineData(HttpStatusCode.TooManyRequests, RustMapsErrorKind.RateLimited)]
    [InlineData(HttpStatusCode.InternalServerError, RustMapsErrorKind.Unknown)]
    public async Task GetMapByIdAsync_MapsStatusCodeToErrorKind(HttpStatusCode status, RustMapsErrorKind expected)
    {
        var handler = new TestHttpMessageHandler(
            status, "{\"meta\":{\"status\":\"Failed\",\"statusCode\":0,\"errors\":[\"boom\"]}}");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("missing");

        Assert.False(result.IsSuccess);
        Assert.Equal(expected, result.Error!.Kind);
        Assert.Equal("boom", result.Error.Message);
    }

    [Fact]
    public async Task GetMapByIdAsync_SuccessWithNoData_FailsAsUnknown()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200}}");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("x");

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
    }

    [Fact]
    public async Task GetMapByIdAsync_NonJsonErrorBody_LeavesMessageNull()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.BadGateway, "<html>down</html>");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("x");

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
        Assert.Null(result.Error.Message);
    }
}
