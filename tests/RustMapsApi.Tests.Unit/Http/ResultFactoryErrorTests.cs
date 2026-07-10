using System.Net;
using System.Net.Http.Headers;
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
        const string body = "{\"meta\":{\"status\":\"Failed\",\"statusCode\":0,\"errors\":[\"boom\"]}}";
        var handler = new TestHttpMessageHandler(status, body);
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("missing");

        Assert.False(result.IsSuccess);
        Assert.Equal(expected, result.Error!.Kind);
        Assert.Equal("boom", result.Error.Message);
        Assert.Equal(body, result.Error.RawBody);
    }

    [Fact]
    public async Task SearchByFilterAsync_ErrorStatus_MapsPagedResponseToFailure()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.TooManyRequests,
            "{\"meta\":{\"status\":\"Failed\",\"statusCode\":0,\"errors\":[\"slow down\"]}}");
        var client = CreateClient(handler);

        var result = await client.SearchByFilterAsync("filter-1", 0);

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.RateLimited, result.Error!.Kind);
        Assert.Equal("slow down", result.Error.Message);
    }

    [Fact]
    public async Task SearchByFilterAsync_SuccessWithNoData_FailsAsUnknown()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200}}");
        var client = CreateClient(handler);

        var result = await client.SearchByFilterAsync("filter-1", 0);

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
    }

    [Fact]
    public async Task SearchByFilterAsync_RateLimited_ParsesRetryAfterHeader()
    {
        var handler = new TestHttpMessageHandler(_ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("{\"meta\":{\"status\":\"Failed\",\"statusCode\":0}}")
            };
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(30));
            return response;
        });
        var client = CreateClient(handler);

        var result = await client.SearchByFilterAsync("filter-1", 0);

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.RateLimited, result.Error!.Kind);
        Assert.Equal(TimeSpan.FromSeconds(30), result.Error.RetryAfter);
    }

    [Fact]
    public async Task GetMapByIdAsync_ErrorBodyWithoutMeta_LeavesMessageNull()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.BadRequest, "{}");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("x");

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Validation, result.Error!.Kind);
        Assert.Null(result.Error.Message);
    }

    [Fact]
    public async Task GetMapByIdAsync_SuccessWithNullBody_FailsAsUnknown()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("x");

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
    }

    [Fact]
    public async Task SearchByFilterAsync_SuccessWithNullBody_FailsAsUnknown()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(handler);

        var result = await client.SearchByFilterAsync("filter-1", 0);

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
    }

    [Fact]
    public async Task GetMapByIdAsync_ErrorWithNullBody_LeavesMessageNull()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.BadRequest, "null");
        var client = CreateClient(handler);

        var result = await client.GetMapByIdAsync("x");

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Validation, result.Error!.Kind);
        Assert.Null(result.Error.Message);
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
