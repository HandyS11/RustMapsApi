using System.Text.Json;
using RustMapsApi.Http;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Serialization;

namespace RustMapsApi.Tests.Unit.V4.Serialization;

public class JsonContextV4Tests
{
    [Fact]
    public void Context_DeserializesMapEnvelopeFixture()
    {
        var options = RustMapsJsonOptions.Create(RustMapsJsonContextV4.Default);
        var json = File.ReadAllText(Path.Combine("Fixtures", "map.json"));

        var envelope = JsonSerializer.Deserialize<ServiceResponse<MapInfo>>(json, options);

        Assert.NotNull(envelope);
        Assert.NotNull(envelope!.Data);
    }
}
