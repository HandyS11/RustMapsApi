using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.V4.Requests;

public partial class RequestModelsTests
{
    [Fact]
    public void MapGenerationRequest_SerializesCamelCase()
    {
        var options = RustMapsJsonOptions.Create(RequestContext.Default);
        var request = new MapGenerationRequest
        {
            Size = 4500, Seed = 12345, Staging = false
        };

        var json = JsonSerializer.Serialize(request, options);

        Assert.Contains("\"size\":4500", json);
        Assert.Contains("\"seed\":12345", json);
        Assert.Contains("\"staging\":false", json);
    }

    [JsonSerializable(typeof(MapGenerationRequest))]
    private sealed partial class RequestContext : JsonSerializerContext;
}
