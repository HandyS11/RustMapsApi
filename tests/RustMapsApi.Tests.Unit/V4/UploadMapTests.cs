using System.Net;
using System.Net.Http;
using System.Text;
using RustMapsApi.V4;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.V4;

public class UploadMapTests
{
    private const string UploadedJson =
        "{\"meta\":{\"status\":\"Success\",\"statusCode\":200}," +
        "\"data\":{\"id\":\"u-1\",\"state\":\"uploading\",\"displayName\":\"My Map\",\"seed\":42}}";

    private static RustMapsClient CreateClient(TestHttpMessageHandler handler) =>
        new(new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        });

    private static MapUpload Upload(string? note) => new()
    {
        Map = new MemoryStream(Encoding.UTF8.GetBytes("MAPDATA")), FileName = "world.map", Staging = true, Note = note,
    };

    [Fact]
    public async Task UploadMapAsync_WithNote_PostsMultipartToUploadRoute()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, UploadedJson);
        var client = CreateClient(handler);

        var result = await client.UploadMapAsync(Upload(note: "first build"));

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal("u-1", result.Data!.Id);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("/v4/maps/upload", handler.LastRequest.RequestUri!.AbsolutePath);

        var body = handler.LastRequestBody!;
        Assert.Contains("name=map", body.Replace("\"", string.Empty));
        Assert.Contains("world.map", body);
        Assert.Contains("staging", body);
        Assert.Contains("true", body);
        Assert.Contains("first build", body);
    }

    [Fact]
    public async Task UploadMapAsync_WithoutNote_OmitsNotePart()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, UploadedJson);
        var client = CreateClient(handler);

        await client.UploadMapAsync(Upload(note: null));

        var body = handler.LastRequestBody!;
        Assert.DoesNotContain("name=note", body.Replace("\"", string.Empty));
    }

    [Fact]
    public async Task UploadMapAsync_NullUpload_Throws()
    {
        var handler = new TestHttpMessageHandler(HttpStatusCode.OK, "{}");
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<ArgumentNullException>(() => client.UploadMapAsync(null!));
    }
}
