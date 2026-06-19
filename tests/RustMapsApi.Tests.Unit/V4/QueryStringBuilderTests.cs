using System.Net;
using System.Net.Http;
using RustMapsApi.V4;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.V4;

public class QueryStringBuilderTests
{
    private const string PagedJson =
        "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":[]}";

    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task NullOptions_EmitsPageOnly()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchByFilterAsync("f", page: 3);

        var query = handler.LastRequest!.RequestUri!.Query;
        Assert.Contains("page=3", query);
        Assert.DoesNotContain("staging", query);
        Assert.DoesNotContain("sortBy", query);
    }

    [Fact]
    public async Task AllOptions_EmitEveryKnownKey()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchByFilterAsync("f", page: 1, new SearchOptions
        {
            Staging = true,
            CustomMaps = false,
            IncludeAllProtocols = true,
            IgnoreVisitedMaps = false,
            SortBy = "createdAt desc",
        });

        var query = handler.LastRequest!.RequestUri!.Query;
        Assert.Contains("staging=true", query);
        Assert.Contains("customMaps=false", query);
        Assert.Contains("includeAllProtocols=true", query);
        Assert.Contains("ignoreVisitedMaps=false", query);
        Assert.Contains("sortBy=createdAt", Uri.UnescapeDataString(query));
    }

    [Fact]
    public async Task EmptySortBy_IsOmitted()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchByFilterAsync("f", page: 0, new SearchOptions { SortBy = "" });

        Assert.DoesNotContain("sortBy", handler.LastRequest!.RequestUri!.Query);
    }
}
