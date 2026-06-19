using System.Net.Http;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.V4.Integration;

public class LiveApiTests
{
    [SkippableFact]
    public async Task GetLimitsAsync_WithRealKey_Succeeds()
    {
        var apiKey = Environment.GetEnvironmentVariable("RUSTMAPS_API_KEY");
        Skip.If(string.IsNullOrWhiteSpace(apiKey), "RUSTMAPS_API_KEY not set.");

        using var http = new HttpClient { BaseAddress = new Uri("https://api.rustmaps.com") };
        http.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        var client = new RustMapsClient(http);

        var result = await client.GetLimitsAsync();

        Assert.True(result.IsSuccess, result.Error?.Message);
    }
}
