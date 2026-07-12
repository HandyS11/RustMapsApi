# RustMapsApi.Assets Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ship a new NuGet package `RustMapsApi.Assets` that bundles RustMaps' monument icon SVGs and maps each `MonumentType` from an API response to its icon, entirely offline.

**Architecture:** A new packable project references the core `RustMapsApi` (for the real `MonumentType` enum), embeds ~42 SVGs as assembly resources, and exposes a static `MonumentAssets` accessor (`TryGetAsset`/`GetAsset`/`HasAsset`/`AvailableTypes`) returning an immutable `MonumentAsset` that opens its embedded SVG on demand. An explicit hand-curated `Dictionary<MonumentType,string>` is the single source of truth for the mapping; optional DI sugar wraps the static API.

**Tech Stack:** C#, .NET (`netstandard2.0;net10.0`), xUnit 2.9.3, `Xunit.SkippableFact`, Central Package Management, embedded resources.

## Global Constraints

- **Target frameworks:** `netstandard2.0;net10.0` for the package (test project: `net10.0`). Verbatim from spec.
- **No `FrozenDictionary`** — not available on `netstandard2.0`; use `Dictionary<MonumentType,string>`.
- **`Directory.Build.props` applies to every project:** `Nullable=enable`, `ImplicitUsings=enable`, `TreatWarningsAsErrors=true`, `GenerateDocumentationFile=true`, `AnalysisLevel=latest-all` (SonarAnalyzer + Roslynator + NetAnalyzers). Consequence: **every public type and member MUST have an XML `/// <summary>` doc comment**, or the build fails (CS1591 as error). All code below includes them.
- **Analyzers fire as errors** (this repo's history): SonarAnalyzer `S1075` (hardcoded URI) is pre-suppressed via a narrow `<NoWarn>` in the package csproj (the CDN base is intentional). If any other analyzer (Sonar/Roslynator/CA) fires as an error, resolve it the way the repo does — a narrow, justified `<NoWarn>`/`.editorconfig`/`[SuppressMessage]` with a comment, or a minimal code adjustment — and note the deviation in your report. Do not disable analyzers broadly.
- **Central Package Management:** all `PackageReference`s are versionless; versions live in `Directory.Packages.props`.
- **Namespace:** library code lives in `RustMapsApi.V4.Assets`; the DI extension lives in `Microsoft.Extensions.DependencyInjection`.
- **RootNamespace** of the package project is `RustMapsApi.Assets`, so embedded resources under `Assets/` are named `RustMapsApi.Assets.Assets.<File>.svg`.
- **Package metadata** mirrors the core lib: MIT, `PackageIcon=icon.png`, `PackageReadmeFile=README.md`, `IncludeSymbols=true`, `SymbolPackageFormat=snupkg`.
- **CDN base:** `https://content.rustmaps.com/assets/<AssetName>.svg`. Probing requires a browser `User-Agent` (Cloudflare returns 403 otherwise).
- **`docs/superpowers/` is git-ignored** — the spec and this plan are local-only; do not attempt to commit them.

**Spec:** `docs/superpowers/specs/2026-07-10-rustmaps-assets-package-design.md`

---

## Reference data: the complete resolved mapping

**42 distinct asset files** (verified `200` on the CDN, 2026-07-10):

```
Gasstation, Supermarket, Warehouse, Lighthouse, Harbor, Airfield, Junkyard,
Launch_Site, Military_Tunnels, Powerplant, Trainyard, Water_Treatment,
Sphere_Tank, Bandit_Town, Sewer_Branch, Satellite_Dish, Outpost, Excavator,
Sulfur_Quarry, Stone_Quarry, Hqm_Quarry, Oilrig_Large, Oilrig_Small,
Fishing_Village, Water_Well, Swamp, Cave, Iceberg, Powerline, Power_Substation,
Large_Barn, Tunnel_Entrance, Tunnel_Entrance_Transition, Underwater_Lab,
Military_Base, Arctic_Research_Base, Nuclear_Missile_Silo, Ferry_Terminal_1,
Radtown, Jungle_Ruin, Ziggurat, Apartments_Complex
```

**Assetless `MonumentType` members** (no icon on the CDN — absent from the map): `Unknown (-1)`, `NotImplemented (0)`, `Mountain1..5`, `IceLake1..4`, `LargeGodRock`, `MediumGodRock`, `TinyGodRock`, `ThreeWallRock`, `AnvilRock`, `LakeA..C`, `CanyonA..C`, `OasisA..C`, `CustomMonument (10000)`.

---

## Task 1: Scaffold the package and test projects

**Files:**

- Create: `src/RustMapsApi.Assets/RustMapsApi.Assets.csproj`
- Create: `src/RustMapsApi.Assets/Assets/.gitkeep`
- Create: `tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
- Create: `tests/RustMapsApi.Assets.Tests.Unit/ScaffoldTests.cs`
- Modify: `Directory.Packages.props` (add one `PackageVersion`)
- Modify: `RustMapsApi.slnx` (add both projects)

**Interfaces:**

- Consumes: nothing.
- Produces: buildable `RustMapsApi.Assets` assembly (namespace `RustMapsApi.V4.Assets`) and a wired-up unit-test project.

- [ ] **Step 1: Create the package project file**

Create `src/RustMapsApi.Assets/RustMapsApi.Assets.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>netstandard2.0;net10.0</TargetFrameworks>
    <IsPackable>true</IsPackable>
    <PackageId>RustMapsApi.Assets</PackageId>
    <RootNamespace>RustMapsApi.Assets</RootNamespace>
    <LangVersion>latest</LangVersion>
    <Description>Monument icon assets (SVG) for the RustMaps API, mapped by MonumentType.</Description>
    <PackageTags>rust;rustmaps;gaming;icons;svg;assets;rust-game</PackageTags>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/HandyS11/RustMapsApi</PackageProjectUrl>
    <RepositoryUrl>https://github.com/HandyS11/RustMapsApi</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>
    <PackageReadmeFile>README.md</PackageReadmeFile>
    <PackageIcon>icon.png</PackageIcon>
    <!-- S1075: this package's entire purpose is the RustMaps CDN base URL
         (content.rustmaps.com/assets); it is intentionally inlined as a constant. -->
    <NoWarn>$(NoWarn);S1075</NoWarn>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../RustMapsApi/RustMapsApi.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" />
    <PackageReference Include="PolySharp">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <EmbeddedResource Include="Assets/*.svg" />
  </ItemGroup>

  <ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
    <None Include="../../icon.png" Pack="true" PackagePath="\" />
  </ItemGroup>

</Project>
```

> Note: `README.md` is packed but not yet created — that is Task 6. The build does not require it to exist; only `dotnet pack` does. A placeholder is unnecessary for `dotnet build`.

- [ ] **Step 2: Create the Assets folder placeholder**

Create `src/RustMapsApi.Assets/Assets/.gitkeep` (empty file). Keeps the folder in git before SVGs land in Task 2; `EmbeddedResource Include="Assets/*.svg"` matching zero files is a valid build.

- [ ] **Step 3: Add the DI abstractions package version**

In `Directory.Packages.props`, inside the first `<ItemGroup>` (the `<!-- Packages -->` group), add:

```xml
    <PackageVersion Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.9" />
```

- [ ] **Step 4: Create the unit-test project file**

Create `tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="coverlet.collector" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\RustMapsApi.Assets\RustMapsApi.Assets.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 5: Add a trivial scaffold test**

Create `tests/RustMapsApi.Assets.Tests.Unit/ScaffoldTests.cs`:

```csharp
namespace RustMapsApi.Assets.Tests.Unit;

public sealed class ScaffoldTests
{
    [Fact]
    public void AssetsAssembly_IsReferenceable()
    {
        var assembly = typeof(RustMapsApi.V4.Models.MonumentType).Assembly;
        Assert.NotNull(assembly);
    }
}
```

- [ ] **Step 6: Wire both projects into the solution**

In `RustMapsApi.slnx`, add the src project under the `/src/` folder and the test project under the `/tests/` folder:

```xml
  <Folder Name="/src/">
    <Project Path="src/RustMapsApi/RustMapsApi.csproj" />
    <Project Path="src/RustMapsApi.Assets/RustMapsApi.Assets.csproj" />
  </Folder>
```

```xml
  <Folder Name="/tests/">
    <Project Path="tests/RustMapsApi.Tests.Integration/RustMapsApi.Tests.Integration.csproj" />
    <Project Path="tests/RustMapsApi.Tests.Unit/RustMapsApi.Tests.Unit.csproj" />
    <Project Path="tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj" />
  </Folder>
```

- [ ] **Step 7: Build and test**

Run: `dotnet build RustMapsApi.slnx -c Debug`
Expected: build succeeds (new projects compile).

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: PASS (1 test).

- [ ] **Step 8: Commit**

```bash
git add src/RustMapsApi.Assets tests/RustMapsApi.Assets.Tests.Unit Directory.Packages.props RustMapsApi.slnx
git commit -m "chore: scaffold RustMapsApi.Assets package and unit-test project"
```

---

## Task 2: Vendor and embed the monument SVGs

**Files:**

- Create: `src/RustMapsApi.Assets/Assets/*.svg` (42 files, downloaded)
- Delete: `src/RustMapsApi.Assets/Assets/.gitkeep` (no longer needed once real files exist)
- Create: `tests/RustMapsApi.Assets.Tests.Unit/EmbeddedResourceTests.cs`

**Interfaces:**

- Consumes: the package project from Task 1.
- Produces: 42 embedded resources named `RustMapsApi.Assets.Assets.<Name>.svg`.

- [ ] **Step 1: Download the 42 SVGs**

Run this script (downloads into the Assets folder with the required browser `User-Agent`, then verifies each file is non-empty and looks like SVG):

```bash
cd /home/handys11/Dev/RustMapsApi
UA="Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0 Safari/537.36"
base="https://content.rustmaps.com/assets/"
dest="src/RustMapsApi.Assets/Assets"
mkdir -p "$dest"
names="Gasstation Supermarket Warehouse Lighthouse Harbor Airfield Junkyard \
Launch_Site Military_Tunnels Powerplant Trainyard Water_Treatment Sphere_Tank \
Bandit_Town Sewer_Branch Satellite_Dish Outpost Excavator Sulfur_Quarry \
Stone_Quarry Hqm_Quarry Oilrig_Large Oilrig_Small Fishing_Village Water_Well \
Swamp Cave Iceberg Powerline Power_Substation Large_Barn Tunnel_Entrance \
Tunnel_Entrance_Transition Underwater_Lab Military_Base Arctic_Research_Base \
Nuclear_Missile_Silo Ferry_Terminal_1 Radtown Jungle_Ruin Ziggurat Apartments_Complex"
fail=0
for n in $names; do
  curl -sS -A "$UA" --max-time 20 -o "$dest/$n.svg" "$base$n.svg"
  if [ ! -s "$dest/$n.svg" ]; then echo "EMPTY: $n.svg"; fail=1; continue; fi
  head -c 200 "$dest/$n.svg" | grep -qiE '<svg|<\?xml' || { echo "NOT SVG: $n.svg"; fail=1; }
done
count=$(ls "$dest"/*.svg | wc -l)
echo "Downloaded $count files (expected 42), fail=$fail"
```

Expected: `Downloaded 42 files (expected 42), fail=0`, no `EMPTY`/`NOT SVG` lines.

- [ ] **Step 2: Remove the placeholder**

```bash
rm -f src/RustMapsApi.Assets/Assets/.gitkeep
```

- [ ] **Step 3: Write the failing embedded-resource test**

Create `tests/RustMapsApi.Assets.Tests.Unit/EmbeddedResourceTests.cs`:

```csharp
using System.Linq;
using System.Reflection;
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class EmbeddedResourceTests
{
    private static Assembly AssetsAssembly => typeof(MonumentAsset).Assembly;

    [Fact]
    public void EmbeddedSvgResources_CountIs42()
    {
        var svgResources = AssetsAssembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".svg", System.StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(42, svgResources.Length);
    }

    [Fact]
    public void EmbeddedSvgResources_AreAllNonEmpty()
    {
        var svgResources = AssetsAssembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".svg", System.StringComparison.Ordinal));

        foreach (var name in svgResources)
        {
            using var stream = AssetsAssembly.GetManifestResourceStream(name);
            Assert.NotNull(stream);
            Assert.True(stream!.Length > 0, $"{name} is empty");
        }
    }
}
```

> This references `MonumentAsset`, which does not exist until Task 3. To keep Task 2 independently testable, temporarily anchor the assembly via a type that already exists:
> replace `typeof(MonumentAsset).Assembly` with `typeof(RustMapsApi.V4.Models.MonumentType).Assembly.GetReferencedAssemblies()`-style lookup is fragile — instead load the Assets assembly by name:

Use this assembly anchor in the test file instead (works before any Assets type exists):

```csharp
    private static Assembly AssetsAssembly =>
        Assembly.Load("RustMapsApi.Assets");
```

Remove the `using RustMapsApi.V4.Assets;` line for now; add it back in Task 3 when `MonumentAsset` exists and you switch the anchor to `typeof(MonumentAsset).Assembly`.

- [ ] **Step 4: Run the tests**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: PASS (3 tests). If `CountIs42` fails, re-check Step 1 output.

- [ ] **Step 5: Commit**

```bash
git add src/RustMapsApi.Assets/Assets tests/RustMapsApi.Assets.Tests.Unit/EmbeddedResourceTests.cs
git commit -m "feat: vendor and embed 42 RustMaps monument icon SVGs"
```

---

## Task 3: The `MonumentAsset` value type

**Files:**

- Create: `src/RustMapsApi.Assets/MonumentAsset.cs`
- Create: `tests/RustMapsApi.Assets.Tests.Unit/MonumentAssetTests.cs`

**Interfaces:**

- Consumes: `RustMapsApi.V4.Models.MonumentType`; embedded resources from Task 2.
- Produces: `public sealed class MonumentAsset` with `internal MonumentAsset(MonumentType, string)`; members `MonumentType MonumentType`, `string AssetName`, `string FileName`, `string MediaType`, `Uri SourceUri`, `Stream OpenStream()`, `byte[] GetBytes()`, `string GetSvg()`.

- [ ] **Step 1: Write the failing test**

Create `tests/RustMapsApi.Assets.Tests.Unit/MonumentAssetTests.cs`:

```csharp
using System;
using System.IO;
using System.Reflection;
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class MonumentAssetTests
{
    // Construct a MonumentAsset via the internal ctor for a known-embedded asset.
    private static MonumentAsset Cave() =>
        (MonumentAsset)Activator.CreateInstance(
            typeof(MonumentAsset),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { MonumentType.CaveLargeHard, "Cave" },
            culture: null)!;

    [Fact]
    public void Metadata_IsDerivedFromAssetName()
    {
        var asset = Cave();
        Assert.Equal(MonumentType.CaveLargeHard, asset.MonumentType);
        Assert.Equal("Cave", asset.AssetName);
        Assert.Equal("Cave.svg", asset.FileName);
        Assert.Equal("image/svg+xml", asset.MediaType);
        Assert.Equal("https://content.rustmaps.com/assets/Cave.svg", asset.SourceUri.ToString());
    }

    [Fact]
    public void OpenStream_ReturnsNonEmptyStream()
    {
        using var stream = Cave().OpenStream();
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void GetSvg_ReturnsSvgMarkup()
    {
        var svg = Cave().GetSvg();
        Assert.False(string.IsNullOrWhiteSpace(svg));
        Assert.Contains("<svg", svg, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void GetBytes_MatchesStreamLength()
    {
        var asset = Cave();
        using var stream = asset.OpenStream();
        Assert.Equal(stream.Length, asset.GetBytes().Length);
    }
}
```

- [ ] **Step 2: Run to verify it fails**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: FAIL — `MonumentAsset` does not exist (compile error).

- [ ] **Step 3: Implement `MonumentAsset`**

Create `src/RustMapsApi.Assets/MonumentAsset.cs`:

```csharp
using System.Text;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>A monument icon asset (SVG) resolved from a <see cref="MonumentType"/>.</summary>
public sealed class MonumentAsset
{
    private const string ResourcePrefix = "RustMapsApi.Assets.Assets.";
    private const string CdnBase = "https://content.rustmaps.com/assets/";

    /// <summary>Creates an asset descriptor. Only the internal map constructs these.</summary>
    /// <param name="monumentType">The monument type this asset was resolved for.</param>
    /// <param name="assetName">The CDN base name of the asset (no extension).</param>
    internal MonumentAsset(MonumentType monumentType, string assetName)
    {
        MonumentType = monumentType;
        AssetName = assetName;
    }

    /// <summary>The monument type this asset was resolved for.</summary>
    public MonumentType MonumentType { get; }

    /// <summary>The RustMaps CDN base name of the asset, e.g. <c>"Nuclear_Missile_Silo"</c>.</summary>
    public string AssetName { get; }

    /// <summary>The asset file name, e.g. <c>"Nuclear_Missile_Silo.svg"</c>.</summary>
    public string FileName => AssetName + ".svg";

    /// <summary>The MIME media type of the asset — always <c>"image/svg+xml"</c>.</summary>
    public string MediaType => "image/svg+xml";

    /// <summary>The canonical RustMaps CDN URL this asset was sourced from.</summary>
    public Uri SourceUri => new(CdnBase + FileName);

    /// <summary>Opens a new read stream over the embedded SVG. The caller owns and disposes it.</summary>
    /// <returns>A readable stream of the SVG bytes.</returns>
    public Stream OpenStream()
    {
        var stream = typeof(MonumentAsset).Assembly
            .GetManifestResourceStream(ResourcePrefix + FileName);
        return stream ?? throw new InvalidOperationException(
            $"Embedded monument asset '{FileName}' was not found.");
    }

    /// <summary>Reads the full SVG as a byte array.</summary>
    /// <returns>The SVG bytes.</returns>
    public byte[] GetBytes()
    {
        using var stream = OpenStream();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }

    /// <summary>Reads the full SVG markup as a UTF-8 string.</summary>
    /// <returns>The SVG markup.</returns>
    public string GetSvg()
    {
        using var stream = OpenStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
```

- [ ] **Step 4: Switch the Task 2 test's assembly anchor**

In `tests/RustMapsApi.Assets.Tests.Unit/EmbeddedResourceTests.cs`, now that `MonumentAsset` exists, replace:

```csharp
    private static Assembly AssetsAssembly =>
        Assembly.Load("RustMapsApi.Assets");
```

with:

```csharp
    private static Assembly AssetsAssembly => typeof(RustMapsApi.V4.Assets.MonumentAsset).Assembly;
```

- [ ] **Step 5: Run to verify pass**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: PASS (all tests).

- [ ] **Step 6: Commit**

```bash
git add src/RustMapsApi.Assets/MonumentAsset.cs tests/RustMapsApi.Assets.Tests.Unit
git commit -m "feat: add MonumentAsset value type with embedded SVG access"
```

---

## Task 4: The mapping and `MonumentAssets` accessor

**Files:**

- Create: `src/RustMapsApi.Assets/MonumentAssetMap.cs`
- Create: `src/RustMapsApi.Assets/MonumentAssets.cs`
- Create: `tests/RustMapsApi.Assets.Tests.Unit/MonumentAssetsTests.cs`
- Create: `tests/RustMapsApi.Assets.Tests.Unit/MapIntegrityTests.cs`

**Interfaces:**

- Consumes: `MonumentAsset` (Task 3); embedded resources (Task 2).
- Produces:
  - `internal static class MonumentAssetMap` with `internal static readonly IReadOnlyDictionary<MonumentType,string> AssetNames`.
  - `public static class MonumentAssets` with `bool TryGetAsset(MonumentType, [MaybeNullWhen(false)] out MonumentAsset)`, `MonumentAsset GetAsset(MonumentType)`, `bool HasAsset(MonumentType)`, `IReadOnlyCollection<MonumentType> AvailableTypes`.

- [ ] **Step 1: Write the failing accessor tests**

Create `tests/RustMapsApi.Assets.Tests.Unit/MonumentAssetsTests.cs`:

```csharp
using System.Collections.Generic;
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class MonumentAssetsTests
{
    [Fact]
    public void TryGetAsset_MappedType_ReturnsTrueAndAsset()
    {
        var ok = MonumentAssets.TryGetAsset(MonumentType.NuclearMissileSilo, out var asset);
        Assert.True(ok);
        Assert.Equal("Nuclear_Missile_Silo", asset.AssetName);
    }

    [Theory]
    [InlineData(MonumentType.CaveLargeHard, "Cave")]
    [InlineData(MonumentType.CaveSmallEasy, "Cave")]
    [InlineData(MonumentType.HarborLarge, "Harbor")]
    [InlineData(MonumentType.StablesA, "Large_Barn")]
    [InlineData(MonumentType.JungleZigguratA, "Ziggurat")]
    [InlineData(MonumentType.OilrigLarge, "Oilrig_Large")]
    [InlineData(MonumentType.OilrigSmall, "Oilrig_Small")]
    [InlineData(MonumentType.TunnelEntranceTransition, "Tunnel_Entrance_Transition")]
    [InlineData(MonumentType.ApartmentsComplex, "Apartments_Complex")]
    public void TryGetAsset_ResolvesExpectedAssetName(MonumentType type, string expected)
    {
        Assert.True(MonumentAssets.TryGetAsset(type, out var asset));
        Assert.Equal(expected, asset.AssetName);
    }

    [Theory]
    [InlineData(MonumentType.Unknown)]
    [InlineData(MonumentType.NotImplemented)]
    [InlineData(MonumentType.CustomMonument)]
    [InlineData(MonumentType.Mountain1)]
    [InlineData(MonumentType.IceLake1)]
    [InlineData(MonumentType.LakeA)]
    [InlineData(MonumentType.CanyonA)]
    [InlineData(MonumentType.OasisA)]
    [InlineData(MonumentType.LargeGodRock)]
    [InlineData(MonumentType.AnvilRock)]
    public void TryGetAsset_AssetlessType_ReturnsFalse(MonumentType type)
    {
        Assert.False(MonumentAssets.TryGetAsset(type, out _));
        Assert.False(MonumentAssets.HasAsset(type));
    }

    [Fact]
    public void GetAsset_AssetlessType_Throws()
    {
        Assert.Throws<KeyNotFoundException>(() => MonumentAssets.GetAsset(MonumentType.Mountain1));
    }

    [Fact]
    public void AvailableTypes_MatchesHasAsset()
    {
        Assert.NotEmpty(MonumentAssets.AvailableTypes);
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            Assert.True(MonumentAssets.HasAsset(type));
        }
    }
}
```

- [ ] **Step 2: Write the failing map-integrity tests**

Create `tests/RustMapsApi.Assets.Tests.Unit/MapIntegrityTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class MapIntegrityTests
{
    private static Assembly AssetsAssembly => typeof(MonumentAsset).Assembly;

    private static IEnumerable<string> EmbeddedAssetNames() =>
        AssetsAssembly.GetManifestResourceNames()
            .Where(n => n.EndsWith(".svg", StringComparison.Ordinal))
            .Select(n => n.Substring("RustMapsApi.Assets.Assets.".Length))
            .Select(n => n.Substring(0, n.Length - ".svg".Length));

    [Fact]
    public void EveryMappedType_ResolvesToNonEmptyStream()
    {
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            Assert.True(MonumentAssets.TryGetAsset(type, out var asset));
            using var stream = asset.OpenStream();
            Assert.True(stream.Length > 0, $"{type} -> {asset.FileName} empty");
        }
    }

    [Fact]
    public void EveryMappedAssetName_HasAnEmbeddedResource()
    {
        var embedded = EmbeddedAssetNames().ToHashSet(StringComparer.Ordinal);
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            MonumentAssets.TryGetAsset(type, out var asset);
            Assert.Contains(asset.AssetName, embedded);
        }
    }

    [Fact]
    public void EveryEmbeddedResource_IsReferencedByTheMap()
    {
        var referenced = MonumentAssets.AvailableTypes
            .Select(t => { MonumentAssets.TryGetAsset(t, out var a); return a.AssetName; })
            .ToHashSet(StringComparer.Ordinal);

        foreach (var embedded in EmbeddedAssetNames())
        {
            Assert.Contains(embedded, referenced);
        }
    }

    [Fact]
    public void DistinctAssetNames_CountIs42()
    {
        var distinct = MonumentAssets.AvailableTypes
            .Select(t => { MonumentAssets.TryGetAsset(t, out var a); return a.AssetName; })
            .Distinct(StringComparer.Ordinal)
            .Count();

        Assert.Equal(42, distinct);
    }
}
```

- [ ] **Step 3: Run to verify they fail**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: FAIL — `MonumentAssets`/`MonumentAssetMap` do not exist (compile error).

- [ ] **Step 4: Implement the map**

Create `src/RustMapsApi.Assets/MonumentAssetMap.cs`:

```csharp
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>The single source of truth mapping each icon-bearing
/// <see cref="MonumentType"/> to its RustMaps CDN asset base name.</summary>
internal static class MonumentAssetMap
{
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
            [MonumentType.HarborSmall] = "Harbor",
            [MonumentType.HarborLarge] = "Harbor",
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
            [MonumentType.FishingVillageA] = "Fishing_Village",
            [MonumentType.FishingVillageB] = "Fishing_Village",
            [MonumentType.FishingVillageC] = "Fishing_Village",
            [MonumentType.WaterWellA] = "Water_Well",
            [MonumentType.WaterWellB] = "Water_Well",
            [MonumentType.WaterWellC] = "Water_Well",
            [MonumentType.WaterWellD] = "Water_Well",
            [MonumentType.WaterWellE] = "Water_Well",
            [MonumentType.SwampA] = "Swamp",
            [MonumentType.SwampB] = "Swamp",
            [MonumentType.SwampC] = "Swamp",
            [MonumentType.CaveLargeHard] = "Cave",
            [MonumentType.CaveLargeMedium] = "Cave",
            [MonumentType.CaveLargeSewersHard] = "Cave",
            [MonumentType.CaveMediumEasy] = "Cave",
            [MonumentType.CaveMediumHard] = "Cave",
            [MonumentType.CaveMediumMedium] = "Cave",
            [MonumentType.CaveSmallEasy] = "Cave",
            [MonumentType.CaveSmallHard] = "Cave",
            [MonumentType.CaveSmallMedium] = "Cave",
            [MonumentType.Iceberg1] = "Iceberg",
            [MonumentType.Iceberg2] = "Iceberg",
            [MonumentType.Iceberg3] = "Iceberg",
            [MonumentType.Iceberg4] = "Iceberg",
            [MonumentType.Iceberg5] = "Iceberg",
            [MonumentType.PowerlineA] = "Powerline",
            [MonumentType.PowerlineB] = "Powerline",
            [MonumentType.PowerlineC] = "Powerline",
            [MonumentType.PowerlineD] = "Powerline",
            [MonumentType.PowerSubstationSmall1] = "Power_Substation",
            [MonumentType.PowerSubstationSmall2] = "Power_Substation",
            [MonumentType.PowerSubstationBig2] = "Power_Substation",
            [MonumentType.PowerSubstationBig1] = "Power_Substation",
            [MonumentType.StablesA] = "Large_Barn",
            [MonumentType.StablesB] = "Large_Barn",
            [MonumentType.TunnelEntrance] = "Tunnel_Entrance",
            [MonumentType.UnderwaterA] = "Underwater_Lab",
            [MonumentType.UnderwaterB] = "Underwater_Lab",
            [MonumentType.UnderwaterC] = "Underwater_Lab",
            [MonumentType.UnderwaterD] = "Underwater_Lab",
            [MonumentType.MilitaryBaseA] = "Military_Base",
            [MonumentType.MilitaryBaseB] = "Military_Base",
            [MonumentType.MilitaryBaseC] = "Military_Base",
            [MonumentType.MilitaryBaseD] = "Military_Base",
            [MonumentType.ArcticResearchBaseA] = "Arctic_Research_Base",
            [MonumentType.NuclearMissileSilo] = "Nuclear_Missile_Silo",
            [MonumentType.FerryTerminal1] = "Ferry_Terminal_1",
            [MonumentType.TunnelEntranceTransition] = "Tunnel_Entrance_Transition",
            [MonumentType.Radtown] = "Radtown",
            [MonumentType.JungleRuinA] = "Jungle_Ruin",
            [MonumentType.JungleRuinB] = "Jungle_Ruin",
            [MonumentType.JungleRuinC] = "Jungle_Ruin",
            [MonumentType.JungleRuinD] = "Jungle_Ruin",
            [MonumentType.JungleRuinE] = "Jungle_Ruin",
            [MonumentType.JungleZigguratA] = "Ziggurat",
            [MonumentType.ApartmentsComplex] = "Apartments_Complex",
        };
}
```

- [ ] **Step 5: Implement the accessor**

Create `src/RustMapsApi.Assets/MonumentAssets.cs`:

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>Resolves RustMaps monument icon assets from a <see cref="MonumentType"/>.</summary>
public static class MonumentAssets
{
    /// <summary>Attempts to resolve the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <param name="asset">The resolved asset when this returns <c>true</c>.</param>
    /// <returns><c>true</c> if the type has an icon; otherwise <c>false</c>.</returns>
    public static bool TryGetAsset(MonumentType type, [MaybeNullWhen(false)] out MonumentAsset asset)
    {
        if (MonumentAssetMap.AssetNames.TryGetValue(type, out var name))
        {
            asset = new MonumentAsset(type, name);
            return true;
        }

        asset = null;
        return false;
    }

    /// <summary>Resolves the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <returns>The resolved asset.</returns>
    /// <exception cref="System.Collections.Generic.KeyNotFoundException">
    /// The type has no icon asset.</exception>
    public static MonumentAsset GetAsset(MonumentType type) =>
        TryGetAsset(type, out var asset)
            ? asset
            : throw new KeyNotFoundException($"No icon asset is mapped for MonumentType.{type}.");

    /// <summary>Indicates whether a monument type has an icon asset.</summary>
    /// <param name="type">The monument type.</param>
    /// <returns><c>true</c> if an icon exists for the type.</returns>
    public static bool HasAsset(MonumentType type) =>
        MonumentAssetMap.AssetNames.ContainsKey(type);

    /// <summary>All monument types that have an icon asset.</summary>
    public static IReadOnlyCollection<MonumentType> AvailableTypes { get; } =
        MonumentAssetMap.AssetNames.Keys.ToArray();
}
```

- [ ] **Step 6: Run to verify pass**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: PASS (all tests, including the 42-distinct-name integrity check).

- [ ] **Step 7: Commit**

```bash
git add src/RustMapsApi.Assets/MonumentAssetMap.cs src/RustMapsApi.Assets/MonumentAssets.cs tests/RustMapsApi.Assets.Tests.Unit
git commit -m "feat: add MonumentType->icon mapping and MonumentAssets accessor"
```

---

## Task 5: Optional dependency-injection sugar

**Files:**

- Create: `src/RustMapsApi.Assets/IMonumentAssetSource.cs`
- Create: `src/RustMapsApi.Assets/MonumentAssetSource.cs`
- Create: `src/RustMapsApi.Assets/DependencyInjection/ServiceCollectionExtensions.cs`
- Create: `tests/RustMapsApi.Assets.Tests.Unit/DependencyInjectionTests.cs`

**Interfaces:**

- Consumes: `MonumentAssets` (Task 4), `MonumentAsset` (Task 3).
- Produces:
  - `public interface IMonumentAssetSource` mirroring the static API.
  - `AddRustMapsAssets(this IServiceCollection)` registering `IMonumentAssetSource` as a singleton.

- [ ] **Step 1: Write the failing DI test**

Create `tests/RustMapsApi.Assets.Tests.Unit/DependencyInjectionTests.cs`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

namespace RustMapsApi.Assets.Tests.Unit;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void AddRustMapsAssets_ResolvesSource_ThatDelegatesToStaticApi()
    {
        var provider = new ServiceCollection()
            .AddRustMapsAssets()
            .BuildServiceProvider();

        var source = provider.GetRequiredService<IMonumentAssetSource>();

        Assert.True(source.HasAsset(MonumentType.Outpost));
        Assert.True(source.TryGetAsset(MonumentType.Outpost, out var asset));
        Assert.Equal("Outpost", asset.AssetName);
        Assert.False(source.HasAsset(MonumentType.Mountain1));
        Assert.NotEmpty(source.AvailableTypes);
    }
}
```

- [ ] **Step 2: Run to verify it fails**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: FAIL — `AddRustMapsAssets`/`IMonumentAssetSource` do not exist (compile error).

- [ ] **Step 3: Implement the interface**

Create `src/RustMapsApi.Assets/IMonumentAssetSource.cs`:

```csharp
using System.Diagnostics.CodeAnalysis;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>An injectable source of RustMaps monument icon assets.</summary>
public interface IMonumentAssetSource
{
    /// <summary>Attempts to resolve the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <param name="asset">The resolved asset when this returns <c>true</c>.</param>
    /// <returns><c>true</c> if the type has an icon; otherwise <c>false</c>.</returns>
    bool TryGetAsset(MonumentType type, [MaybeNullWhen(false)] out MonumentAsset asset);

    /// <summary>Resolves the icon asset for a monument type.</summary>
    /// <param name="type">The monument type from an API response.</param>
    /// <returns>The resolved asset.</returns>
    MonumentAsset GetAsset(MonumentType type);

    /// <summary>Indicates whether a monument type has an icon asset.</summary>
    /// <param name="type">The monument type.</param>
    /// <returns><c>true</c> if an icon exists for the type.</returns>
    bool HasAsset(MonumentType type);

    /// <summary>All monument types that have an icon asset.</summary>
    IReadOnlyCollection<MonumentType> AvailableTypes { get; }
}
```

- [ ] **Step 4: Implement the source**

Create `src/RustMapsApi.Assets/MonumentAssetSource.cs`:

```csharp
using System.Diagnostics.CodeAnalysis;
using RustMapsApi.V4.Models;

namespace RustMapsApi.V4.Assets;

/// <summary>Default <see cref="IMonumentAssetSource"/> delegating to <see cref="MonumentAssets"/>.</summary>
public sealed class MonumentAssetSource : IMonumentAssetSource
{
    /// <inheritdoc />
    public bool TryGetAsset(MonumentType type, [MaybeNullWhen(false)] out MonumentAsset asset) =>
        MonumentAssets.TryGetAsset(type, out asset);

    /// <inheritdoc />
    public MonumentAsset GetAsset(MonumentType type) => MonumentAssets.GetAsset(type);

    /// <inheritdoc />
    public bool HasAsset(MonumentType type) => MonumentAssets.HasAsset(type);

    /// <inheritdoc />
    public IReadOnlyCollection<MonumentType> AvailableTypes => MonumentAssets.AvailableTypes;
}
```

- [ ] **Step 5: Implement the DI extension**

Create `src/RustMapsApi.Assets/DependencyInjection/ServiceCollectionExtensions.cs`:

```csharp
using RustMapsApi.V4.Assets;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registration helpers for RustMaps monument icon assets.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IMonumentAssetSource"/> as a singleton.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    public static IServiceCollection AddRustMapsAssets(this IServiceCollection services)
    {
        services.AddSingleton<IMonumentAssetSource, MonumentAssetSource>();
        return services;
    }
}
```

- [ ] **Step 6: Run to verify pass**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Unit/RustMapsApi.Assets.Tests.Unit.csproj`
Expected: PASS (all tests).

- [ ] **Step 7: Commit**

```bash
git add src/RustMapsApi.Assets/IMonumentAssetSource.cs src/RustMapsApi.Assets/MonumentAssetSource.cs src/RustMapsApi.Assets/DependencyInjection tests/RustMapsApi.Assets.Tests.Unit/DependencyInjectionTests.cs
git commit -m "feat: add optional DI registration for monument assets"
```

---

## Task 6: Package README and attribution

**Files:**

- Create: `src/RustMapsApi.Assets/README.md`

**Interfaces:**

- Consumes: the public API from Tasks 3-5.
- Produces: the `PackageReadmeFile` that ships to nuget.org.

- [ ] **Step 1: Write the README**

Create `src/RustMapsApi.Assets/README.md`:

````markdown
# RustMapsApi.Assets

Monument icon artwork (SVG) for the [RustMaps API](https://rustmaps.com), mapped by `MonumentType`.
A companion to [`RustMapsApi`](https://www.nuget.org/packages/RustMapsApi): given a monument from an
API response, get its icon with no network call.

## Install

```bash
dotnet add package RustMapsApi.Assets
```

## Usage

```csharp
using RustMapsApi.V4.Assets;
using RustMapsApi.V4.Models;

if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
{
    // asset.AssetName  -> "Nuclear_Missile_Silo"
    // asset.FileName   -> "Nuclear_Missile_Silo.svg"
    // asset.MediaType  -> "image/svg+xml"
    // asset.SourceUri  -> https://content.rustmaps.com/assets/Nuclear_Missile_Silo.svg
    string svg = asset.GetSvg();       // markup
    byte[] bytes = asset.GetBytes();   // raw bytes
    using Stream stream = asset.OpenStream();
}
```

Types with no published icon (terrain such as `Mountain*`/`Lake*`, and the
`Unknown`/`NotImplemented`/`CustomMonument` sentinels) return `false` from `TryGetAsset`.
`GetAsset` throws `KeyNotFoundException` for those; `HasAsset` and `AvailableTypes` let you
query coverage.

### WPF

WPF cannot render SVG natively. Feed `OpenStream()` to an SVG renderer such as
[SharpVectors](https://www.nuget.org/packages/SharpVectors.Wpf) or
[SkiaSharp](https://www.nuget.org/packages/SkiaSharp), e.g. with SharpVectors:

```csharp
using var stream = MonumentAssets.GetAsset(monument.Type).OpenStream();
var settings = new WpfDrawingSettings();
using var reader = new FileSvgReader(settings);
DrawingGroup drawing = reader.Read(stream);
var image = new DrawingImage(drawing); // bind to an <Image Source="..." />
```

### Dependency injection (optional)

```csharp
services.AddRustMapsAssets();           // registers IMonumentAssetSource
```

## Attribution

The monument icon artwork is © [RustMaps](https://rustmaps.com) and served from their public CDN
(`content.rustmaps.com`). This package redistributes those icons unmodified for convenience alongside
the RustMaps API. All rights to the artwork remain with RustMaps. The package code is MIT-licensed.
````

- [ ] **Step 2: Verify the package builds with the README**

Run: `dotnet pack src/RustMapsApi.Assets/RustMapsApi.Assets.csproj -c Release`
Expected: `Successfully created package '.../RustMapsApi.Assets.<version>.nupkg'`.

- [ ] **Step 3: Verify README and icon are packed**

Run: `unzip -l $(ls -t artifacts/**/RustMapsApi.Assets.*.nupkg 2>/dev/null | head -1 || find . -name 'RustMapsApi.Assets.*.nupkg' | head -1) | grep -E 'README|icon|\.svg'`
Expected: lists `README.md`, `icon.png`, and 42 `.svg` entries under the package.

- [ ] **Step 4: Commit**

```bash
git add src/RustMapsApi.Assets/README.md
git commit -m "docs: add RustMapsApi.Assets package README and attribution"
```

---

## Task 7 (optional): Live CDN drift-detection integration test

Only build this if you want CI/manual detection of upstream asset changes. It is network-gated and
skipped by default, mirroring the repo's `RUSTMAPS_API_KEY` skip convention.

**Files:**

- Create: `tests/RustMapsApi.Assets.Tests.Integration/RustMapsApi.Assets.Tests.Integration.csproj`
- Create: `tests/RustMapsApi.Assets.Tests.Integration/CdnDriftTests.cs`
- Modify: `RustMapsApi.slnx` (add the project)

**Interfaces:**

- Consumes: `MonumentAssets.AvailableTypes` and `MonumentAsset.SourceUri`.
- Produces: a skippable test asserting each `SourceUri` returns `200`.

- [ ] **Step 1: Create the integration test project**

Create `tests/RustMapsApi.Assets.Tests.Integration/RustMapsApi.Assets.Tests.Integration.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <Using Include="Xunit" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
    <PackageReference Include="xunit" />
    <PackageReference Include="xunit.runner.visualstudio" />
    <PackageReference Include="Xunit.SkippableFact" />
    <PackageReference Include="coverlet.collector" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\RustMapsApi.Assets\RustMapsApi.Assets.csproj" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Write the skippable drift test**

Create `tests/RustMapsApi.Assets.Tests.Integration/CdnDriftTests.cs`:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RustMapsApi.V4.Assets;
using Xunit;

namespace RustMapsApi.Assets.Tests.Integration;

public sealed class CdnDriftTests
{
    private const string Ua =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/120.0 Safari/537.36";

    private static bool LiveEnabled =>
        Environment.GetEnvironmentVariable("RUSTMAPS_ASSETS_LIVE") == "1";

    public static IEnumerable<object[]> DistinctSourceUris()
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var type in MonumentAssets.AvailableTypes)
        {
            MonumentAssets.TryGetAsset(type, out var asset);
            if (seen.Add(asset.AssetName))
            {
                yield return new object[] { asset.SourceUri };
            }
        }
    }

    [SkippableTheory]
    [MemberData(nameof(DistinctSourceUris))]
    public async Task SourceUri_StillReturns200(Uri uri)
    {
        Skip.IfNot(LiveEnabled, "Set RUSTMAPS_ASSETS_LIVE=1 to run live CDN drift checks.");

        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", Ua);
        using var request = new HttpRequestMessage(HttpMethod.Head, uri);
        using var response = await http.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

- [ ] **Step 3: Wire into the solution**

In `RustMapsApi.slnx`, add under the `/tests/` folder:

```xml
    <Project Path="tests/RustMapsApi.Assets.Tests.Integration/RustMapsApi.Assets.Tests.Integration.csproj" />
```

- [ ] **Step 4: Verify it builds and skips by default**

Run: `dotnet test tests/RustMapsApi.Assets.Tests.Integration/RustMapsApi.Assets.Tests.Integration.csproj`
Expected: all 42 theory cases reported as **skipped** (no `RUSTMAPS_ASSETS_LIVE`).

Run (optional live): `RUSTMAPS_ASSETS_LIVE=1 dotnet test tests/RustMapsApi.Assets.Tests.Integration/RustMapsApi.Assets.Tests.Integration.csproj`
Expected: PASS (42 cases).

- [ ] **Step 5: Commit**

```bash
git add tests/RustMapsApi.Assets.Tests.Integration RustMapsApi.slnx
git commit -m "test: add skippable live CDN drift check for monument assets"
```

---

## Final verification

- [ ] Run the full solution build and test:

Run: `dotnet build RustMapsApi.slnx -c Release`
Expected: succeeds with zero warnings (TreatWarningsAsErrors is on).

Run: `dotnet test RustMapsApi.slnx -c Release`
Expected: all unit tests pass; integration drift cases skip.

- [ ] Confirm the package contents one final time:

Run: `dotnet pack src/RustMapsApi.Assets/RustMapsApi.Assets.csproj -c Release` and inspect the `.nupkg` for 42 SVGs, `README.md`, and `icon.png`.

---

## Self-review notes (traceability to spec)

- **SVG-only, embedded** → Task 1 (`EmbeddedResource`), Task 2 (vendor + embed).
- **New package referencing RustMapsApi** → Task 1 (`ProjectReference`).
- **Try+Get, not HTTP `Result<T>`** → Task 4 (`TryGetAsset`/`GetAsset`, `KeyNotFoundException`).
- **Hand-written explicit map** → Task 4 (`MonumentAssetMap.AssetNames`). *Refinement vs spec's
  "switch expression": a `Dictionary` is used instead so `AvailableTypes` and integrity checks derive
  from one source with no drift — still an explicit, hand-written map, `netstandard2.0`-safe.*
- **Partial coverage / assetless types** → Task 4 tests (assetless theory) + reference-data list.
- **Optional DI sugar** → Task 5.
- **Offline integrity tests + optional live drift test** → Task 4 (integrity), Task 7 (drift).
- **Attribution + WPF doc** → Task 6 README.
- **Discovery** → already completed during planning; the concrete 42-name mapping is baked into
  Task 2's download list and Task 4's dictionary, so no runtime discovery remains.
