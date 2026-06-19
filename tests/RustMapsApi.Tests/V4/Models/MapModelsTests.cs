namespace RustMapsApi.Tests.V4.Models;

using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;

public partial class MapModelsTests
{
    [JsonSerializable(typeof(MapThumbnail))]
    [JsonSerializable(typeof(MapState))]
    private sealed partial class ModelsContext : JsonSerializerContext;

    [Fact]
    public void MapThumbnail_DeserializesCamelCaseWireFormat()
    {
        var options = RustMapsJsonOptions.Create(ModelsContext.Default);
        const string json = "{\"mapId\":\"abc\",\"seed\":123,\"size\":4500,\"url\":\"https://x\"}";

        var thumbnail = JsonSerializer.Deserialize<MapThumbnail>(json, options);

        Assert.NotNull(thumbnail);
        Assert.Equal("abc", thumbnail!.MapId);
        Assert.Equal(123, thumbnail.Seed);
        Assert.Equal(4500, thumbnail.Size);
    }

    [Fact]
    public void MapState_DeserializesFromString()
    {
        var options = RustMapsJsonOptions.Create(ModelsContext.Default);

        var state = JsonSerializer.Deserialize<MapState>("\"inQueue\"", options);

        Assert.Equal(MapState.InQueue, state);
    }
}
