using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Http;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;
using RustMapsApi.V4.Serialization;

namespace RustMapsApi.Tests.Unit.V4;

public class RecordCoverageTests
{
    private static JsonSerializerOptions Options() =>
        RustMapsJsonOptions.Create(
            RustMapsJsonContextV4.Default,
            new JsonNumberEnumConverter<BiomeType>(),
            new JsonNumberEnumConverter<MonumentType>());

    [Fact]
    public void MapSettings_DeserializesIdAndName()
    {
        const string json = """{"data":{"id":"cfg-42","name":"My Config","settings":null},"meta":{"status":"success","statusCode":200}}""";
        var options = Options();

        var envelope = JsonSerializer.Deserialize<ServiceResponse<MapSettings>>(json, options);

        Assert.NotNull(envelope);
        var settings = envelope!.Data;
        Assert.NotNull(settings);
        Assert.Equal("cfg-42", settings!.Id);
        Assert.Equal("My Config", settings.Name);
        Assert.Null(settings.Settings);
    }

    [Fact]
    public void MapSettings_DeserializesNestedSettings()
    {
        const string json = """{"data":{"id":"cfg-99","name":"Full","settings":{"removeCarWrecks":true}},"meta":{"status":"success","statusCode":200}}""";
        var options = Options();

        var envelope = JsonSerializer.Deserialize<ServiceResponse<MapSettings>>(json, options);

        Assert.NotNull(envelope);
        var settings = envelope!.Data;
        Assert.NotNull(settings);
        Assert.Equal("cfg-99", settings!.Id);
        Assert.NotNull(settings.Settings);
        Assert.True(settings.Settings!.RemoveCarWrecks);
    }

    [Fact]
    public void CustomPrefab_DeserializesEnabledAndId()
    {
        const string json = """{"enabled":true,"id":"prefab-abc"}""";

        var prefab = JsonSerializer.Deserialize<CustomPrefab>(json, Options());

        Assert.NotNull(prefab);
        Assert.True(prefab!.Enabled);
        Assert.Equal("prefab-abc", prefab.Id);
    }

    [Fact]
    public void CustomPrefab_DisabledWithNullId_Deserializes()
    {
        const string json = """{"enabled":false}""";

        var prefab = JsonSerializer.Deserialize<CustomPrefab>(json, Options());

        Assert.NotNull(prefab);
        Assert.False(prefab!.Enabled);
        Assert.Null(prefab.Id);
    }

    [Fact]
    public void MonumentBiomePreference_DeserializesBiomeTypeAndSelection()
    {
        const string json = """{"biomeType":8,"selection":"wanted"}""";

        var pref = JsonSerializer.Deserialize<MonumentBiomePreference>(json, Options());

        Assert.NotNull(pref);
        Assert.Equal(BiomeType.Forest, pref!.BiomeType);
        Assert.Equal(SelectionStatus.Wanted, pref.Selection);
    }

    [Fact]
    public void MonumentBiomePreference_NotWanted_Deserializes()
    {
        const string json = """{"biomeType":4,"selection":"notWanted"}""";

        var pref = JsonSerializer.Deserialize<MonumentBiomePreference>(json, Options());

        Assert.NotNull(pref);
        Assert.Equal(BiomeType.Desert, pref!.BiomeType);
        Assert.Equal(SelectionStatus.NotWanted, pref.Selection);
    }

    [Fact]
    public void PrefabCustomizableMonumentConfiguration_DeserializesCustomPrefabAndBase()
    {
        const string json = """{"type":90,"blocked":false,"allowedToSetBiomes":true,"desired":true,"customPrefab":{"enabled":true,"id":"prefab-xyz"}}""";

        var config = JsonSerializer.Deserialize<PrefabCustomizableMonumentConfiguration>(json, Options());

        Assert.NotNull(config);
        Assert.Equal(MonumentType.Outpost, config!.Type);
        Assert.False(config.Blocked);
        Assert.True(config.AllowedToSetBiomes);
        Assert.True(config.Desired);
        Assert.NotNull(config.CustomPrefab);
        Assert.True(config.CustomPrefab!.Enabled);
        Assert.Equal("prefab-xyz", config.CustomPrefab.Id);
    }

    [Fact]
    public void CreateCustomMapFromConfigRequest_SerializesRoundTrip()
    {
        var request = new CreateCustomMapFromConfigRequest
        {
            OrgId = "org-99",
            ConfigName = "my-config",
            MapParameters = new MapGenerationRequest { Size = 3500, Seed = 777, Staging = false },
        };
        var options = Options();

        var json = JsonSerializer.Serialize(request, options);
        var deserialized = JsonSerializer.Deserialize<CreateCustomMapFromConfigRequest>(json, options);

        Assert.NotNull(deserialized);
        Assert.Equal("my-config", deserialized!.ConfigName);
        Assert.Equal(3500, deserialized.MapParameters.Size);
        Assert.Equal(777, deserialized.MapParameters.Seed);
        Assert.False(deserialized.MapParameters.Staging);
        Assert.Null(deserialized.OrgId);
    }

    [Fact]
    public void MinMaxFilter_StoresMinAndMax()
    {
        var filter = new MinMaxFilter(2, 8);

        Assert.Equal(2, filter.Min);
        Assert.Equal(8, filter.Max);
    }

    [Fact]
    public void MinMaxFilter_EqualityOnSameValues()
    {
        var a = new MinMaxFilter(1, 5);
        var b = new MinMaxFilter(1, 5);

        Assert.Equal(a, b);
    }

    [Fact]
    public void MinMaxFilter_InequalityOnDifferentValues()
    {
        var a = new MinMaxFilter(1, 5);
        var b = new MinMaxFilter(1, 6);

        Assert.NotEqual(a, b);
    }
}
