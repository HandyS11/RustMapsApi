using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.V4.Models;

public partial class EnumWireFormatTests
{
    private static JsonSerializerOptions Options() => RustMapsJsonOptions.Create(
        EnumContext.Default,
        new JsonNumberEnumConverter<BiomeType>(),
        new JsonNumberEnumConverter<MonumentType>());

    [Fact]
    public void BiomeType_SerializesAsInteger()
    {
        var json = JsonSerializer.Serialize(new BiomeFilter
        {
            Type = BiomeType.Forest
        }, Options());

        Assert.Contains("\"type\":8", json);
        Assert.DoesNotContain("forest", json);
    }

    [Fact]
    public void BiomeType_DeserializesFromInteger()
    {
        var filter = JsonSerializer.Deserialize<BiomeFilter>("{\"type\":16}", Options());

        Assert.Equal(BiomeType.Tundra, filter!.Type);
    }

    [Fact]
    public void MonumentType_SerializesAsInteger()
    {
        var json = JsonSerializer.Serialize(
            new MonumentFilter
            {
                Type = MonumentType.LaunchSite, SelectionStatus = SelectionStatus.NoPreference
            },
            Options());

        Assert.Contains("\"type\":45", json);
        Assert.DoesNotContain("launchSite", json);
    }

    [Fact]
    public void StringEnums_StillSerializeAsCamelCaseNames()
    {
        var state = JsonSerializer.Serialize(MapState.InQueue, Options());
        var selection = JsonSerializer.Serialize(SelectionStatus.NoPreference, Options());

        Assert.Equal("\"inQueue\"", state);
        Assert.Equal("\"noPreference\"", selection);
    }

    [JsonSerializable(typeof(BiomeFilter))]
    [JsonSerializable(typeof(MonumentFilter))]
    [JsonSerializable(typeof(MapState))]
    [JsonSerializable(typeof(SelectionStatus))]
    private sealed partial class EnumContext : JsonSerializerContext;
}
