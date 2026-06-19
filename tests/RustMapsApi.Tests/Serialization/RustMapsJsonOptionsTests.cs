using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Serialization;

namespace RustMapsApi.Tests.Serialization;

public partial class RustMapsJsonOptionsTests
{
    [Fact]
    public void Create_UsesCamelCase_AndOmitsNulls()
    {
        var options = RustMapsJsonOptions.Create(ProbeContext.Default);

        var json = JsonSerializer.Serialize(new Probe
        {
            ExampleName = "x"
        }, typeof(Probe), options);

        Assert.Contains("\"exampleName\":\"x\"", json);
        Assert.DoesNotContain("maybe", json);
    }

    private sealed class Probe
    {
        public string ExampleName { get; set; } = "";

        public string? Maybe { get; set; }
    }

    [JsonSerializable(typeof(Probe))]
    private sealed partial class ProbeContext : JsonSerializerContext;
}
