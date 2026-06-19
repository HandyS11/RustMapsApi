namespace RustMapsApi.V4.Models;

/// <summary>Configuration for an oil rig monument.</summary>
public sealed record OilRigConfiguration : LargeMonumentConfiguration
{
    /// <summary>The preferred biome for the rig.</summary>
    public BiomePreference? BiomePreference { get; init; }

    /// <summary>The explicit placement of the rig.</summary>
    public OilRigPosition? Position { get; init; }
}
