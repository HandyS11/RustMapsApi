namespace RustMapsApi.V4.Models;

/// <summary>The full settings tree describing a custom map.</summary>
public sealed record CustomMapSettings
{
    /// <summary>Whether to generate the ring road.</summary>
    public SelectionStatus? GenerateRingRoad { get; init; }

    /// <summary>Whether to generate above-ground train tracks.</summary>
    public SelectionStatus? GenerateAboveGroundTrainTracks { get; init; }

    /// <summary>Whether to remove small power lines.</summary>
    public bool? RemoveSmallPowerLines { get; init; }

    /// <summary>Whether to remove large power lines.</summary>
    public bool? RemoveLargePowerLines { get; init; }

    /// <summary>Whether to remove car wrecks.</summary>
    public bool? RemoveCarWrecks { get; init; }

    /// <summary>Whether to remove rivers.</summary>
    public bool? RemoveRivers { get; init; }

    /// <summary>Whether building on roads is allowed.</summary>
    public bool? AllowBuildingOnRoads { get; init; }

    /// <summary>Whether to modify loot tiers.</summary>
    public bool? ModifyTiers { get; init; }

    /// <summary>Whether to try spawning the outpost in the centre.</summary>
    public bool? TrySpawningOutpostInCenter { get; init; }

    /// <summary>The terrain configuration.</summary>
    public TerrainConfiguration? TerrainConfiguration { get; init; }

    /// <summary>The oil-rig configurations.</summary>
    public IReadOnlyList<OilRigConfiguration>? OilRigConfigurations { get; init; }

    /// <summary>The safe-zone configurations.</summary>
    public IReadOnlyList<PrefabCustomizableMonumentConfiguration>? Safezones { get; init; }

    /// <summary>The large-monument configurations.</summary>
    public IReadOnlyList<LargeMonumentConfiguration>? LargeMonuments { get; init; }

    /// <summary>The small-monument configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? SmallMonuments { get; init; }

    /// <summary>The harbor configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? Harbors { get; init; }

    /// <summary>The water-well configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? WaterWells { get; init; }

    /// <summary>The cave configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? Caves { get; init; }

    /// <summary>The mountain configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? Mountains { get; init; }

    /// <summary>The quarry configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? Quarries { get; init; }

    /// <summary>The iceberg configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? Icebergs { get; init; }

    /// <summary>The ice-lake configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? IceLakes { get; init; }

    /// <summary>The ruin configurations.</summary>
    public IReadOnlyList<BasicMonumentConfiguration>? Ruins { get; init; }

    /// <summary>Localised file names keyed by language code.</summary>
    public IReadOnlyDictionary<string, string>? FileName { get; init; }

    /// <summary>The webhook notification settings.</summary>
    public WebhookSettings? Webhook { get; init; }

    /// <summary>The underwater-labs configuration.</summary>
    public LabConfiguration? UnderwaterLabsConfiguration { get; init; }

    /// <summary>The lakes configuration.</summary>
    public LabConfiguration? LakesConfiguration { get; init; }

    /// <summary>The oases configuration.</summary>
    public LabConfiguration? OasesConfiguration { get; init; }

    /// <summary>The canyons configuration.</summary>
    public LabConfiguration? CanyonsConfiguration { get; init; }

    /// <summary>Prefab identifiers to block from spawning.</summary>
    public IReadOnlyList<string>? BlockedPrefabs { get; init; }

    /// <summary>Whether to remove underground tunnels.</summary>
    public bool? RemoveUndergroundTunnels { get; init; }

    /// <summary>Whether to embed the cargo-ship path.</summary>
    public bool? EmbedCargoShipPath { get; init; }
}
