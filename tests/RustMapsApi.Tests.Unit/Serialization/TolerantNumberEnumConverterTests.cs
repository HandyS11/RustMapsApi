using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Tests.Unit.Serialization;

public partial class TolerantNumberEnumConverterTests
{
    private static JsonSerializerOptions Options() => RustMapsJsonOptions.Create(
        ConverterContext.Default,
        new TolerantNumberEnumConverter<BiomeType>(BiomeType.Unknown),
        new TolerantNumberEnumConverter<MonumentType>(MonumentType.Unknown));

    [Fact]
    public void MonumentType_UnknownWireValue_MapsToUnknown()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":99999}", Options());

        Assert.Equal(MonumentType.Unknown, monument!.Type);
    }

    [Fact]
    public void MonumentType_KnownWireValue_StillDeserializes()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":45}", Options());

        Assert.Equal(MonumentType.LaunchSite, monument!.Type);
    }

    [Fact]
    public void MonumentType_StringValue_MapsToUnknown()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":\"foo\"}", Options());

        Assert.Equal(MonumentType.Unknown, monument!.Type);
    }

    [Fact]
    public void MonumentType_NullValue_MapsToUnknown()
    {
        var monument = JsonSerializer.Deserialize<Monument>("{\"type\":null}", Options());

        Assert.Equal(MonumentType.Unknown, monument!.Type);
    }

    [Fact]
    public void BiomeType_UnknownWireValue_MapsToUnknown()
    {
        var filter = JsonSerializer.Deserialize<BiomeFilter>("{\"type\":9999}", Options());

        Assert.Equal(BiomeType.Unknown, filter!.Type);
    }

    [Fact]
    public void MonumentType_KnownValue_SerializesAsInteger()
    {
        var json = JsonSerializer.Serialize(new Monument { Type = MonumentType.LaunchSite }, Options());

        Assert.Contains("\"type\":45", json);
    }

    [Fact]
    public void MonumentType_Unknown_SerializesAsNegativeOne()
    {
        var json = JsonSerializer.Serialize(new Monument { Type = MonumentType.Unknown }, Options());

        Assert.Contains("\"type\":-1", json);
    }

    [JsonSerializable(typeof(Monument))]
    [JsonSerializable(typeof(BiomeFilter))]
    private sealed partial class ConverterContext : JsonSerializerContext;
}
