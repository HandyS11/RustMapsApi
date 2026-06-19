namespace RustMapsApi.V4.Models;

/// <summary>A Rust monument type. Values match the RustMaps wire protocol.</summary>
public enum MonumentType
{
    /// <summary>The NotImplemented monument type (wire value 0).</summary>
    NotImplemented = 0,

    /// <summary>The Gasstation monument type (wire value 5).</summary>
    Gasstation = 5,

    /// <summary>The Supermarket monument type (wire value 10).</summary>
    Supermarket = 10,

    /// <summary>The Warehouse monument type (wire value 15).</summary>
    Warehouse = 15,

    /// <summary>The Lighthouse monument type (wire value 20).</summary>
    Lighthouse = 20,

    /// <summary>The HarborSmall monument type (wire value 25).</summary>
    HarborSmall = 25,

    /// <summary>The HarborLarge monument type (wire value 30).</summary>
    HarborLarge = 30,

    /// <summary>The Airfield monument type (wire value 35).</summary>
    Airfield = 35,

    /// <summary>The Junkyard monument type (wire value 40).</summary>
    Junkyard = 40,

    /// <summary>The LaunchSite monument type (wire value 45).</summary>
    LaunchSite = 45,

    /// <summary>The MilitaryTunnels monument type (wire value 50).</summary>
    MilitaryTunnels = 50,

    /// <summary>The Powerplant monument type (wire value 55).</summary>
    Powerplant = 55,

    /// <summary>The Trainyard monument type (wire value 60).</summary>
    Trainyard = 60,

    /// <summary>The WaterTreatment monument type (wire value 65).</summary>
    WaterTreatment = 65,

    /// <summary>The SphereTank monument type (wire value 70).</summary>
    SphereTank = 70,

    /// <summary>The BanditTown monument type (wire value 75).</summary>
    BanditTown = 75,

    /// <summary>The SewerBranch monument type (wire value 80).</summary>
    SewerBranch = 80,

    /// <summary>The SatelliteDish monument type (wire value 85).</summary>
    SatelliteDish = 85,

    /// <summary>The Outpost monument type (wire value 90).</summary>
    Outpost = 90,

    /// <summary>The Excavator monument type (wire value 95).</summary>
    Excavator = 95,

    /// <summary>The SulfurQuarry monument type (wire value 100).</summary>
    SulfurQuarry = 100,

    /// <summary>The StoneQuarry monument type (wire value 105).</summary>
    StoneQuarry = 105,

    /// <summary>The HqmQuarry monument type (wire value 110).</summary>
    HqmQuarry = 110,

    /// <summary>The OilrigLarge monument type (wire value 115).</summary>
    OilrigLarge = 115,

    /// <summary>The OilrigSmall monument type (wire value 120).</summary>
    OilrigSmall = 120,

    /// <summary>The FishingVillageA monument type (wire value 140).</summary>
    FishingVillageA = 140,

    /// <summary>The FishingVillageB monument type (wire value 145).</summary>
    FishingVillageB = 145,

    /// <summary>The FishingVillageC monument type (wire value 150).</summary>
    FishingVillageC = 150,

    /// <summary>The WaterWellA monument type (wire value 155).</summary>
    WaterWellA = 155,

    /// <summary>The WaterWellB monument type (wire value 160).</summary>
    WaterWellB = 160,

    /// <summary>The WaterWellC monument type (wire value 165).</summary>
    WaterWellC = 165,

    /// <summary>The WaterWellD monument type (wire value 170).</summary>
    WaterWellD = 170,

    /// <summary>The WaterWellE monument type (wire value 175).</summary>
    WaterWellE = 175,

    /// <summary>The Mountain1 monument type (wire value 180).</summary>
    Mountain1 = 180,

    /// <summary>The Mountain2 monument type (wire value 185).</summary>
    Mountain2 = 185,

    /// <summary>The Mountain3 monument type (wire value 190).</summary>
    Mountain3 = 190,

    /// <summary>The Mountain4 monument type (wire value 195).</summary>
    Mountain4 = 195,

    /// <summary>The Mountain5 monument type (wire value 200).</summary>
    Mountain5 = 200,

    /// <summary>The SwampA monument type (wire value 205).</summary>
    SwampA = 205,

    /// <summary>The SwampB monument type (wire value 210).</summary>
    SwampB = 210,

    /// <summary>The SwampC monument type (wire value 215).</summary>
    SwampC = 215,

    /// <summary>The IceLake1 monument type (wire value 220).</summary>
    IceLake1 = 220,

    /// <summary>The IceLake2 monument type (wire value 225).</summary>
    IceLake2 = 225,

    /// <summary>The IceLake3 monument type (wire value 230).</summary>
    IceLake3 = 230,

    /// <summary>The IceLake4 monument type (wire value 235).</summary>
    IceLake4 = 235,

    /// <summary>The CaveLargeHard monument type (wire value 240).</summary>
    CaveLargeHard = 240,

    /// <summary>The CaveLargeMedium monument type (wire value 245).</summary>
    CaveLargeMedium = 245,

    /// <summary>The CaveLargeSewersHard monument type (wire value 250).</summary>
    CaveLargeSewersHard = 250,

    /// <summary>The CaveMediumEasy monument type (wire value 255).</summary>
    CaveMediumEasy = 255,

    /// <summary>The CaveMediumHard monument type (wire value 260).</summary>
    CaveMediumHard = 260,

    /// <summary>The CaveMediumMedium monument type (wire value 265).</summary>
    CaveMediumMedium = 265,

    /// <summary>The CaveSmallEasy monument type (wire value 270).</summary>
    CaveSmallEasy = 270,

    /// <summary>The CaveSmallHard monument type (wire value 275).</summary>
    CaveSmallHard = 275,

    /// <summary>The CaveSmallMedium monument type (wire value 280).</summary>
    CaveSmallMedium = 280,

    /// <summary>The Iceberg1 monument type (wire value 285).</summary>
    Iceberg1 = 285,

    /// <summary>The Iceberg2 monument type (wire value 290).</summary>
    Iceberg2 = 290,

    /// <summary>The Iceberg3 monument type (wire value 295).</summary>
    Iceberg3 = 295,

    /// <summary>The Iceberg4 monument type (wire value 300).</summary>
    Iceberg4 = 300,

    /// <summary>The Iceberg5 monument type (wire value 305).</summary>
    Iceberg5 = 305,

    /// <summary>The PowerlineA monument type (wire value 310).</summary>
    PowerlineA = 310,

    /// <summary>The PowerlineB monument type (wire value 315).</summary>
    PowerlineB = 315,

    /// <summary>The PowerlineC monument type (wire value 320).</summary>
    PowerlineC = 320,

    /// <summary>The PowerlineD monument type (wire value 325).</summary>
    PowerlineD = 325,

    /// <summary>The PowerSubstationSmall1 monument type (wire value 360).</summary>
    PowerSubstationSmall1 = 360,

    /// <summary>The PowerSubstationSmall2 monument type (wire value 365).</summary>
    PowerSubstationSmall2 = 365,

    /// <summary>The PowerSubstationBig2 monument type (wire value 370).</summary>
    PowerSubstationBig2 = 370,

    /// <summary>The PowerSubstationBig1 monument type (wire value 375).</summary>
    PowerSubstationBig1 = 375,

    /// <summary>The StablesA monument type (wire value 380).</summary>
    StablesA = 380,

    /// <summary>The StablesB monument type (wire value 385).</summary>
    StablesB = 385,

    /// <summary>The TunnelEntrance monument type (wire value 390).</summary>
    TunnelEntrance = 390,

    /// <summary>The UnderwaterA monument type (wire value 395).</summary>
    UnderwaterA = 395,

    /// <summary>The UnderwaterB monument type (wire value 400).</summary>
    UnderwaterB = 400,

    /// <summary>The UnderwaterC monument type (wire value 405).</summary>
    UnderwaterC = 405,

    /// <summary>The UnderwaterD monument type (wire value 410).</summary>
    UnderwaterD = 410,

    /// <summary>The MilitaryBaseA monument type (wire value 415).</summary>
    MilitaryBaseA = 415,

    /// <summary>The MilitaryBaseB monument type (wire value 420).</summary>
    MilitaryBaseB = 420,

    /// <summary>The MilitaryBaseC monument type (wire value 425).</summary>
    MilitaryBaseC = 425,

    /// <summary>The MilitaryBaseD monument type (wire value 430).</summary>
    MilitaryBaseD = 430,

    /// <summary>The ArcticResearchBaseA monument type (wire value 435).</summary>
    ArcticResearchBaseA = 435,

    /// <summary>The NuclearMissileSilo monument type (wire value 440).</summary>
    NuclearMissileSilo = 440,

    /// <summary>The FerryTerminal1 monument type (wire value 445).</summary>
    FerryTerminal1 = 445,

    /// <summary>The TunnelEntranceTransition monument type (wire value 450).</summary>
    TunnelEntranceTransition = 450,

    /// <summary>The LargeGodRock monument type (wire value 455).</summary>
    LargeGodRock = 455,

    /// <summary>The MediumGodRock monument type (wire value 460).</summary>
    MediumGodRock = 460,

    /// <summary>The TinyGodRock monument type (wire value 465).</summary>
    TinyGodRock = 465,

    /// <summary>The ThreeWallRock monument type (wire value 470).</summary>
    ThreeWallRock = 470,

    /// <summary>The AnvilRock monument type (wire value 475).</summary>
    AnvilRock = 475,

    /// <summary>The Radtown monument type (wire value 480).</summary>
    Radtown = 480,

    /// <summary>The LakeA monument type (wire value 485).</summary>
    LakeA = 485,

    /// <summary>The LakeB monument type (wire value 490).</summary>
    LakeB = 490,

    /// <summary>The LakeC monument type (wire value 495).</summary>
    LakeC = 495,

    /// <summary>The CanyonA monument type (wire value 500).</summary>
    CanyonA = 500,

    /// <summary>The CanyonB monument type (wire value 505).</summary>
    CanyonB = 505,

    /// <summary>The CanyonC monument type (wire value 510).</summary>
    CanyonC = 510,

    /// <summary>The OasisA monument type (wire value 515).</summary>
    OasisA = 515,

    /// <summary>The OasisB monument type (wire value 520).</summary>
    OasisB = 520,

    /// <summary>The OasisC monument type (wire value 525).</summary>
    OasisC = 525,

    /// <summary>The JungleRuinA monument type (wire value 530).</summary>
    JungleRuinA = 530,

    /// <summary>The JungleRuinB monument type (wire value 535).</summary>
    JungleRuinB = 535,

    /// <summary>The JungleRuinC monument type (wire value 540).</summary>
    JungleRuinC = 540,

    /// <summary>The JungleRuinD monument type (wire value 545).</summary>
    JungleRuinD = 545,

    /// <summary>The JungleRuinE monument type (wire value 550).</summary>
    JungleRuinE = 550,

    /// <summary>The JungleZigguratA monument type (wire value 555).</summary>
    JungleZigguratA = 555,

    /// <summary>The CustomMonument monument type (wire value 10000).</summary>
    CustomMonument = 10000,
}
