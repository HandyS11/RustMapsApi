using System.Text.Json;
using System.Text.Json.Serialization;
using RustMapsApi.Http;
using RustMapsApi.Serialization;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Serialization;

namespace RustMapsApi.Tests.Unit.V4;

public class CustomMapSettingsRoundTripTests
{
    private static JsonSerializerOptions Options() =>
        RustMapsJsonOptions.Create(
            RustMapsJsonContextV4.Default,
            new JsonNumberEnumConverter<BiomeType>(),
            new JsonNumberEnumConverter<MonumentType>());

    [Fact]
    public void CustomMapSettings_DeserializesNestedConfigTree()
    {
        var json = File.ReadAllText(Path.Combine("Fixtures", "custom-map-settings.json"));

        var envelope = JsonSerializer.Deserialize<ServiceResponse<CustomMapSettings>>(json, Options());

        Assert.NotNull(envelope);
        var settings = envelope!.Data;
        Assert.NotNull(settings);

        // Scalars on the root record.
        Assert.True(settings!.RemoveCarWrecks);
        Assert.True(settings.AllowBuildingOnRoads);

        // Terrain -> biome nested config.
        Assert.NotNull(settings.TerrainConfiguration);
        Assert.NotNull(settings.TerrainConfiguration!.BiomeConfig);
        Assert.True(settings.TerrainConfiguration.BiomeConfig!.Enabled);
        Assert.Equal(0.25f, settings.TerrainConfiguration.BiomeConfig.AridPercentage);

        // Oil rig list (derived record) + position.
        Assert.NotNull(settings.OilRigConfigurations);
        var rig = Assert.Single(settings.OilRigConfigurations!);
        Assert.NotNull(rig.Position);
        Assert.True(rig.Position!.Enabled);

        // Lab-style config.
        Assert.NotNull(settings.LakesConfiguration);
        Assert.Equal(2, settings.LakesConfiguration!.MaxAmount);

        // Webhook.
        Assert.NotNull(settings.Webhook);
        Assert.True(settings.Webhook!.Enabled);
        Assert.Equal("https://hooks.example.com/rust", settings.Webhook.Url);

        // Basic monument list.
        Assert.NotNull(settings.SmallMonuments);
        Assert.NotEmpty(settings.SmallMonuments!);
    }
}
