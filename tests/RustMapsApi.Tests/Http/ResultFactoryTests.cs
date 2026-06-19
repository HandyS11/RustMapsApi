using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Http;
using RustMapsApi.Results;
using RustMapsApi.Serialization;

namespace RustMapsApi.Tests.Http;

public partial class ResultFactoryTests
{
    [JsonSerializable(typeof(ServiceResponse<string>))]
    [JsonSerializable(typeof(ServiceResponse<object>))]
    private sealed partial class TestContext : JsonSerializerContext;

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
}
