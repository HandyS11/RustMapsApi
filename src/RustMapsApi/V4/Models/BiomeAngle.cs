namespace RustMapsApi.V4.Models;

/// <summary>The orientation of the biome axis across the map.</summary>
public enum BiomeAngle
{
    /// <summary>Snow at the top, desert at the bottom.</summary>
    TopSnowBottomDesert = 0,

    /// <summary>Desert on the left, snow on the right.</summary>
    LeftDesertRightSnow = 1,

    /// <summary>Desert at the top, snow at the bottom.</summary>
    TopDesertBottomSnow = 2,

    /// <summary>Snow on the left, desert on the right.</summary>
    LeftSnowRightDesert = 3,

    /// <summary>The default orientation.</summary>
    Default = 4,
}
