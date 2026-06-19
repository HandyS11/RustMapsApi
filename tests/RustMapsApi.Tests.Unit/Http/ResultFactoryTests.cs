using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Http;
using RustMapsApi.Results;
using RustMapsApi.Serialization;

namespace RustMapsApi.Tests.Unit.Http;

public partial class ResultFactoryTests
{
    private static JsonSerializerOptions Options() => RustMapsJsonOptions.Create(TestContext.Default);

    [Fact]
    public async Task FromResponseAsync_Success_ReturnsData()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":\"hello\"}"),
        };

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Data);
        Assert.Equal(200, result.StatusCode);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound, RustMapsErrorKind.NotFound)]
    [InlineData(HttpStatusCode.Unauthorized, RustMapsErrorKind.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden, RustMapsErrorKind.Forbidden)]
    [InlineData(HttpStatusCode.Conflict, RustMapsErrorKind.Queued)]
    [InlineData(HttpStatusCode.BadRequest, RustMapsErrorKind.Validation)]
    [InlineData(HttpStatusCode.TooManyRequests, RustMapsErrorKind.RateLimited)]
    public async Task FromResponseAsync_NonSuccess_MapsErrorKind(HttpStatusCode status, RustMapsErrorKind expected)
    {
        using var response = new HttpResponseMessage(status)
        {
            Content = new StringContent("{}"),
        };

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        Assert.Equal(expected, result.Error!.Kind);
        Assert.Equal((int)status, result.StatusCode);
    }

    [Fact]
    public async Task FromResponseAsync_SuccessWithoutData_ReturnsNoDataMessage()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"meta\":{\"status\":\"Success\",\"statusCode\":200}}"),
        };

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
        Assert.Equal("Response contained no data.", result.Error.Message);
    }

    [Fact]
    public async Task FromResponseAsync_MultipleErrors_JoinsWithSemicolon()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                "{\"meta\":{\"status\":\"Failed\",\"statusCode\":400,\"errors\":[\"first\",\"second\"]}}"),
        };

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.Equal("first; second", result.Error!.Message);
    }

    [Fact]
    public async Task FromResponseAsync_EmptyErrorsArray_LeavesMessageNull()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"meta\":{\"status\":\"Failed\",\"statusCode\":400,\"errors\":[]}}"),
        };

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.Null(result.Error!.Message);
    }

    [Fact]
    public async Task FromResponseAsync_MalformedErrorBody_LeavesMessageNull()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("not json at all"),
        };

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Error!.Message);
    }

    [Fact]
    public async Task FromPagedResponseAsync_SuccessWithoutData_ReturnsNoDataMessage()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"meta\":{\"status\":\"Success\",\"statusCode\":200}}"),
        };

        var result = await ResultFactory.FromPagedResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(RustMapsErrorKind.Unknown, result.Error!.Kind);
        Assert.Equal("Response contained no data.", result.Error.Message);
    }

    [Fact]
    public async Task FromPagedResponseAsync_Success_ReturnsList()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content =
                new StringContent("{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":[\"a\",\"b\"]}"),
        };

        var result = await ResultFactory.FromPagedResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data!.Count);
    }

    [Fact]
    public async Task FromResponseAsync_RateLimited_ParsesRetryAfter()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent("{}"),
        };
        response.Headers.Add("Retry-After", "30");

        var result = await ResultFactory.FromResponseAsync<string>(response, Options(), CancellationToken.None);

        Assert.Equal(TimeSpan.FromSeconds(30), result.Error!.RetryAfter);
    }

    [JsonSerializable(typeof(ServiceResponse<string>))]
    [JsonSerializable(typeof(ServiceResponse<object>))]
    [JsonSerializable(typeof(PagedServiceResponse<IReadOnlyList<string>>))]
    private sealed partial class TestContext : JsonSerializerContext;
}
