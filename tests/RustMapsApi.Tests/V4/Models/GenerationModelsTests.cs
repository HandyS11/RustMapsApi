using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Tests.V4.Models;

public partial class GenerationModelsTests
{
    [Fact]
    public void MapGenerationStatus_DeserializesStateAndQueue()
    {
        var options = RustMapsJsonOptions.Create(GenContext.Default);
        const string json =
            "{\"mapId\":\"abc\",\"queuePosition\":3,\"state\":\"generating\",\"currentStep\":\"terrain\"}";

        var status = JsonSerializer.Deserialize<MapGenerationStatus>(json, options);

        Assert.Equal("abc", status!.MapId);
        Assert.Equal(3, status.QueuePosition);
        Assert.Equal(MapState.Generating, status.State);
        Assert.Equal("terrain", status.CurrentStep);
    }

    [Fact]
    public void MapGenLimits_DeserializesConcurrentAndMonthly()
    {
        var options = RustMapsJsonOptions.Create(GenContext.Default);
        const string json =
            "{\"concurrent\":{\"current\":1,\"allowed\":5},\"monthly\":{\"current\":10,\"allowed\":100}}";

        var limits = JsonSerializer.Deserialize<MapGenLimits>(json, options);

        Assert.Equal(1, limits!.Concurrent!.Current);
        Assert.Equal(100, limits.Monthly!.Allowed);
    }

    [JsonSerializable(typeof(MapGenerationStatus))]
    [JsonSerializable(typeof(MapGenLimits))]
    private sealed partial class GenContext : JsonSerializerContext;
}
