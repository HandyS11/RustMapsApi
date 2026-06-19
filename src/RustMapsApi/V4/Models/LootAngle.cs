namespace RustMapsApi.V4.Models;

/// <summary>The orientation of the loot-tier axis across the map.</summary>
public enum LootAngle
{
    /// <summary>Tier 2 at the top, tier 0 at the bottom.</summary>
    TopTier2BottomTier0 = 0,

    /// <summary>Tier 2 on the left, tier 0 on the right.</summary>
    LeftTier2RightTier0 = 1,

    /// <summary>Tier 0 at the top, tier 2 at the bottom.</summary>
    TopTier0BottomTier2 = 2,

    /// <summary>Tier 0 on the left, tier 2 on the right.</summary>
    LeftTier0RightTier2 = 3,

    /// <summary>The default orientation.</summary>
    Default = 4,
}
