using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>The single source of truth mapping each icon-bearing
/// <see cref="MonumentType"/> to its RustMaps CDN asset base name.</summary>
internal static class MonumentAssetMap
{
    private const string Harbor = "Harbor";
    private const string FishingVillage = "Fishing_Village";
    private const string WaterWell = "Water_Well";
    private const string Swamp = "Swamp";
    private const string Cave = "Cave";
    private const string Iceberg = "Iceberg";
    private const string Powerline = "Powerline";
    private const string PowerSubstation = "Power_Substation";
    private const string LargeBarn = "Large_Barn";
    private const string UnderwaterLab = "Underwater_Lab";
    private const string MilitaryBase = "Military_Base";
    private const string JungleRuin = "Jungle_Ruin";

    /// <summary>Maps a <see cref="MonumentType"/> to its asset base name. Types absent from this
    /// map have no published icon (terrain features and the Unknown/NotImplemented/CustomMonument
    /// sentinels).</summary>
    internal static readonly IReadOnlyDictionary<MonumentType, string> AssetNames =
        new Dictionary<MonumentType, string>
        {
            [MonumentType.Gasstation] = "Gasstation",
            [MonumentType.Supermarket] = "Supermarket",
            [MonumentType.Warehouse] = "Warehouse",
            [MonumentType.Lighthouse] = "Lighthouse",
            [MonumentType.HarborSmall] = Harbor,
            [MonumentType.HarborLarge] = Harbor,
            [MonumentType.Airfield] = "Airfield",
            [MonumentType.Junkyard] = "Junkyard",
            [MonumentType.LaunchSite] = "Launch_Site",
            [MonumentType.MilitaryTunnels] = "Military_Tunnels",
            [MonumentType.Powerplant] = "Powerplant",
            [MonumentType.Trainyard] = "Trainyard",
            [MonumentType.WaterTreatment] = "Water_Treatment",
            [MonumentType.SphereTank] = "Sphere_Tank",
            [MonumentType.BanditTown] = "Bandit_Town",
            [MonumentType.SewerBranch] = "Sewer_Branch",
            [MonumentType.SatelliteDish] = "Satellite_Dish",
            [MonumentType.Outpost] = "Outpost",
            [MonumentType.Excavator] = "Excavator",
            [MonumentType.SulfurQuarry] = "Sulfur_Quarry",
            [MonumentType.StoneQuarry] = "Stone_Quarry",
            [MonumentType.HqmQuarry] = "Hqm_Quarry",
            [MonumentType.OilrigLarge] = "Oilrig_Large",
            [MonumentType.OilrigSmall] = "Oilrig_Small",
            [MonumentType.FishingVillageA] = FishingVillage,
            [MonumentType.FishingVillageB] = FishingVillage,
            [MonumentType.FishingVillageC] = FishingVillage,
            [MonumentType.WaterWellA] = WaterWell,
            [MonumentType.WaterWellB] = WaterWell,
            [MonumentType.WaterWellC] = WaterWell,
            [MonumentType.WaterWellD] = WaterWell,
            [MonumentType.WaterWellE] = WaterWell,
            [MonumentType.SwampA] = Swamp,
            [MonumentType.SwampB] = Swamp,
            [MonumentType.SwampC] = Swamp,
            [MonumentType.CaveLargeHard] = Cave,
            [MonumentType.CaveLargeMedium] = Cave,
            [MonumentType.CaveLargeSewersHard] = Cave,
            [MonumentType.CaveMediumEasy] = Cave,
            [MonumentType.CaveMediumHard] = Cave,
            [MonumentType.CaveMediumMedium] = Cave,
            [MonumentType.CaveSmallEasy] = Cave,
            [MonumentType.CaveSmallHard] = Cave,
            [MonumentType.CaveSmallMedium] = Cave,
            [MonumentType.Iceberg1] = Iceberg,
            [MonumentType.Iceberg2] = Iceberg,
            [MonumentType.Iceberg3] = Iceberg,
            [MonumentType.Iceberg4] = Iceberg,
            [MonumentType.Iceberg5] = Iceberg,
            [MonumentType.PowerlineA] = Powerline,
            [MonumentType.PowerlineB] = Powerline,
            [MonumentType.PowerlineC] = Powerline,
            [MonumentType.PowerlineD] = Powerline,
            [MonumentType.PowerSubstationSmall1] = PowerSubstation,
            [MonumentType.PowerSubstationSmall2] = PowerSubstation,
            [MonumentType.PowerSubstationBig2] = PowerSubstation,
            [MonumentType.PowerSubstationBig1] = PowerSubstation,
            [MonumentType.StablesA] = LargeBarn,
            [MonumentType.StablesB] = LargeBarn,
            [MonumentType.TunnelEntrance] = "Tunnel_Entrance",
            [MonumentType.UnderwaterA] = UnderwaterLab,
            [MonumentType.UnderwaterB] = UnderwaterLab,
            [MonumentType.UnderwaterC] = UnderwaterLab,
            [MonumentType.UnderwaterD] = UnderwaterLab,
            [MonumentType.MilitaryBaseA] = MilitaryBase,
            [MonumentType.MilitaryBaseB] = MilitaryBase,
            [MonumentType.MilitaryBaseC] = MilitaryBase,
            [MonumentType.MilitaryBaseD] = MilitaryBase,
            [MonumentType.ArcticResearchBaseA] = "Arctic_Research_Base",
            [MonumentType.NuclearMissileSilo] = "Nuclear_Missile_Silo",
            [MonumentType.FerryTerminal1] = "Ferry_Terminal_1",
            [MonumentType.TunnelEntranceTransition] = "Tunnel_Entrance_Transition",
            [MonumentType.Radtown] = "Radtown",
            [MonumentType.JungleRuinA] = JungleRuin,
            [MonumentType.JungleRuinB] = JungleRuin,
            [MonumentType.JungleRuinC] = JungleRuin,
            [MonumentType.JungleRuinD] = JungleRuin,
            [MonumentType.JungleRuinE] = JungleRuin,
            [MonumentType.JungleZigguratA] = "Ziggurat",
            [MonumentType.ApartmentsComplex] = "Apartments_Complex",
        };
}
