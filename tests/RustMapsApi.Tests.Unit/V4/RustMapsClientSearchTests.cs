using System.Net;
using System.Net.Http;
using RustMapsApi.V4;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.V4;

public class RustMapsClientSearchTests
{
    private const string PagedJson =
        "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":[{\"mapId\":\"a\",\"seed\":1,\"size\":4500}]}";

    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    [Fact]
    public async Task SearchByFilterAsync_BuildsPagedFilterRoute()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        var result = await client.SearchByFilterAsync("filter-1", page: 2, new SearchOptions
        {
            CustomMaps = true
        });

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data!);
        Assert.Equal("/v4/maps/filter/filter-1", handler.LastRequest!.RequestUri!.AbsolutePath);
        Assert.Contains("page=2", handler.LastRequest.RequestUri.Query);
        Assert.Contains("customMaps=true", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task SearchAsync_PostsBodyWithPagedRoute()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        var result = await client.SearchAsync(new SearchQuery(), page: 0);

        Assert.True(result.IsSuccess);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/v4/maps/search", handler.LastRequest.RequestUri!.AbsolutePath);
        Assert.Contains("page=0", handler.LastRequest.RequestUri.Query);
    }

    [Fact]
    public async Task SearchAsync_SerializesSearchQueryIntoBody()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchAsync(new SearchQuery
        {
            Size = new MinMaxFilter(4500, 4500)
        }, page: 0);

        Assert.NotNull(handler.LastRequest!.Content);
        Assert.NotNull(handler.LastRequestBody);
        Assert.Contains("searchQuery", handler.LastRequestBody);
        Assert.Contains("4500", handler.LastRequestBody);
    }

    [Fact]
    public async Task SearchAsync_WithOrgId_SendsOrgHeader()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchAsync(new SearchQuery(), page: 0, orgId: "org-9");

        Assert.True(handler.LastRequest!.Headers.Contains("x-org-id"));
    }

    [Fact]
    public async Task SearchAsync_WithoutOrgId_OmitsOrgHeader()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchAsync(new SearchQuery(), page: 0);

        Assert.False(handler.LastRequest!.Headers.Contains("x-org-id"));
    }

    [Fact]
    public async Task SearchByFilterAsync_WithOrgId_SendsOrgHeader()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchByFilterAsync("filter-1", page: 0, orgId: "org-2");

        Assert.True(handler.LastRequest!.Headers.Contains("x-org-id"));
    }

    [Fact]
    public async Task SearchByFilterAsync_WithoutOrgId_OmitsOrgHeader()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, PagedJson);
        var client = CreateClient(handler);

        await client.SearchByFilterAsync("filter-1", page: 0);

        Assert.False(handler.LastRequest!.Headers.Contains("x-org-id"));
    }

    [Fact]
    public async Task SearchAsync_EmptyDataArray_SucceedsWithEmptyList()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200},\"data\":[]}");
        var client = CreateClient(handler);

        var result = await client.SearchAsync(new SearchQuery(), page: 0);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task SearchAsync_MissingData_Fails()
    {
        var handler = new TestHttpMessageHandler(
            HttpStatusCode.OK, "{\"meta\":{\"status\":\"Success\",\"statusCode\":200}}");
        var client = CreateClient(handler);

        var result = await client.SearchAsync(new SearchQuery(), page: 0);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
    }
}
