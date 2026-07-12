# Assets docs + icon-gallery sample — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Document the `RustMapsApi.Assets` package across README + docs site, and add a runnable
cross-platform Avalonia desktop sample that renders every bundled monument icon offline.

**Architecture:** A new `net10.0` Avalonia app (`samples/RustMapsApi.Assets.GalleryApp`) builds a
`ServiceCollection`, calls `AddRustMapsAssets()`, injects `IMonumentAssetSource`, enumerates
`AvailableTypes`, and renders each icon (SVG) via `Avalonia.Svg.Skia` in a scrollable grid. Docs
work is straight Markdown edits plus one new DocFX guide article.

**Tech Stack:** .NET 10, Avalonia 11.x (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`,
`Avalonia.Svg.Skia`), `Microsoft.Extensions.DependencyInjection`, DocFX.

## Global Constraints

- **Target framework:** `net10.0`; `OutputType=WinExe` for the sample; `IsPackable=false` (inherited).
- **Central Package Management:** `ManagePackageVersionsCentrally=true` — csproj `PackageReference`s
  carry **no** `Version`; every version lives in `Directory.Packages.props`. Prefer
  `dotnet add package <id>` (writes the resolved `PackageVersion` centrally + a versionless
  reference). If NuGet restore is offline, add the `PackageVersion` by hand pinned to the current
  Avalonia 11.x line (e.g. `11.3.0`; `Avalonia.Svg.Skia` tracks the same Avalonia version number).
- **Sample gate opt-outs (this project only):** the sample csproj sets
  `GenerateDocumentationFile=false` (public UI types are not an API surface → no CS1591) and
  `TreatWarningsAsErrors=false` (Avalonia XAML-generated code + the full analyzer stack —
  NetAnalyzers `latest-all`, Roslynator, SonarAnalyzer — would otherwise turn UI-pattern warnings
  into build errors). Analyzers still **run**; warnings just stay non-fatal. Sonar server analysis
  already excludes `**/samples/**`.
- **Format gate:** CI runs `dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"`
  then `git diff --exit-code`. It reformats the sample's `.cs`/`.axaml` (docs `.md` are not in the
  solution, so untouched). Run it locally and commit its output before pushing.
- **Markdown:** only `MD013`, `MD033`, `MD041` are disabled (`.markdownlint.json`). Keep every other
  rule satisfied — notably `MD036` (no bold-as-heading: use `###`) and `MD040` (fence languages).
- **SVG rendering is isolated:** all `Avalonia.Svg.Skia` API use lives in `SvgImageFactory.cs`. The
  app must obtain icon bytes from the **package** (`MonumentAsset.OpenStream()`), never re-embed the
  SVGs — that is the whole point of the demo.
- **No changes** to `src/RustMapsApi.Assets/**` package code. **No test project** for the sample
  (samples are untested by design; they are not referenced by any test project, so coverlet and
  Stryker skip them — verified by the existing three samples).
- **Docs build:** `dotnet docfx docs/docfx.json` must succeed. DocFX generates the API reference from
  `src/**/*.csproj`, so `xref:RustMapsApi.V4.Assets.*` UIDs resolve.

---

### Task 1: Buildable Avalonia app skeleton + solution registration

A blank Avalonia window that builds green as part of the solution. No DI, no Assets yet.

**Files:**

- Create: `samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj`
- Create: `samples/RustMapsApi.Assets.GalleryApp/Program.cs`
- Create: `samples/RustMapsApi.Assets.GalleryApp/App.axaml`
- Create: `samples/RustMapsApi.Assets.GalleryApp/App.axaml.cs`
- Create: `samples/RustMapsApi.Assets.GalleryApp/MainWindow.axaml`
- Create: `samples/RustMapsApi.Assets.GalleryApp/MainWindow.axaml.cs`
- Modify: `RustMapsApi.slnx` (register the project under `/samples/`)
- Modify: `Directory.Packages.props` (Avalonia package versions — written by `dotnet add package`)

**Interfaces:**

- Produces: an executable Avalonia `Application` (`App`) whose `MainWindow` is `MainWindow`; the
  `BuildAvaloniaApp()` entry point in `Program`. Later tasks set `MainWindow.DataContext`.

- [ ] **Step 1: Create the project file**

`samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
    <!-- UI sample: App/Window/view-model types are not a documented API surface. -->
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
    <!-- Avalonia XAML-generated code + the analyzer stack would otherwise fail the build
         under the repo-wide TreatWarningsAsErrors. Analyzers still run; warnings stay non-fatal. -->
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" />
    <PackageReference Include="Avalonia.Desktop" />
    <PackageReference Include="Avalonia.Themes.Fluent" />
  </ItemGroup>

</Project>
```

- [ ] **Step 2: Pin the Avalonia package versions centrally**

Run (writes `PackageVersion` entries into `Directory.Packages.props`):

```bash
dotnet add samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj package Avalonia
dotnet add samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj package Avalonia.Desktop
dotnet add samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj package Avalonia.Themes.Fluent
```

If offline, instead add to `Directory.Packages.props` under a new `<!-- Samples (UI) -->` group:

```xml
<PackageVersion Include="Avalonia" Version="11.3.0" />
<PackageVersion Include="Avalonia.Desktop" Version="11.3.0" />
<PackageVersion Include="Avalonia.Themes.Fluent" Version="11.3.0" />
```

Note: `dotnet add package` may re-add a `Version` attribute to the csproj `PackageReference`; if it
does, delete that attribute so CPM stays authoritative.

- [ ] **Step 3: Create the entry point**

`samples/RustMapsApi.Assets.GalleryApp/Program.cs`:

```csharp
using Avalonia;

namespace RustMapsApi.Assets.GalleryApp;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect();
}
```

- [ ] **Step 4: Create the application + theme**

`samples/RustMapsApi.Assets.GalleryApp/App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="RustMapsApi.Assets.GalleryApp.App">
  <Application.Styles>
    <FluentTheme />
  </Application.Styles>
</Application>
```

`samples/RustMapsApi.Assets.GalleryApp/App.axaml.cs`:

```csharp
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace RustMapsApi.Assets.GalleryApp;

public sealed class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
```

- [ ] **Step 5: Create the (empty) main window**

`samples/RustMapsApi.Assets.GalleryApp/MainWindow.axaml`:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Class="RustMapsApi.Assets.GalleryApp.MainWindow"
        Width="900" Height="640"
        Title="RustMaps Monument Icons">
</Window>
```

`samples/RustMapsApi.Assets.GalleryApp/MainWindow.axaml.cs`:

```csharp
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace RustMapsApi.Assets.GalleryApp;

public sealed partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
```

- [ ] **Step 6: Register the project in the solution**

In `RustMapsApi.slnx`, add inside the `<Folder Name="/samples/">` block (keep alphabetical order):

```xml
    <Project Path="samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj" />
```

- [ ] **Step 7: Restore and build the solution**

Run:

```bash
dotnet build RustMapsApi.slnx -c Release
```

Expected: build succeeds (the new project builds; existing projects unaffected). If `dotnet restore`
cannot reach NuGet in this environment, stop and report it — do not claim the build passed; CI will
build it (see final verification note). If it builds, optionally launch it (a window titled
"RustMaps Monument Icons" opens on a machine with a display; headless environments skip this).

- [ ] **Step 8: Format and commit**

```bash
dotnet tool restore
dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"
git diff --exit-code   # must be clean AFTER staging the reformatted files
git add samples/RustMapsApi.Assets.GalleryApp RustMapsApi.slnx Directory.Packages.props
git commit -m "feat(samples): scaffold Avalonia icon-gallery app"
```

---

### Task 2: Wire DI + view models (icon names, no images yet)

Inject `IMonumentAssetSource`, enumerate `AvailableTypes`, and show each monument's type name in the
window. Proves the DI path end-to-end before rendering is added.

**Files:**

- Modify: `samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj` (add refs)
- Modify: `samples/RustMapsApi.Assets.GalleryApp/App.axaml.cs` (build the provider, set DataContext)
- Create: `samples/RustMapsApi.Assets.GalleryApp/ViewModels/MainWindowViewModel.cs`
- Create: `samples/RustMapsApi.Assets.GalleryApp/ViewModels/MonumentIconViewModel.cs`
- Modify: `samples/RustMapsApi.Assets.GalleryApp/MainWindow.axaml` (bind the list)
- Modify: `Directory.Packages.props` (Microsoft.Extensions.DependencyInjection — via `dotnet add`)

**Interfaces:**

- Consumes: `Microsoft.Extensions.DependencyInjection.AddRustMapsAssets()` (from the Assets package)
  and `RustMapsApi.V4.Assets.IMonumentAssetSource` with members `AvailableTypes`
  (`IReadOnlyCollection<MonumentType>`) and `GetAsset(MonumentType) : MonumentAsset`.
- Produces: `MainWindowViewModel` with `IReadOnlyList<MonumentIconViewModel> Icons` and
  `string Header`; `MonumentIconViewModel` with `string TypeName` and `string AssetName`.

- [ ] **Step 1: Add the project + DI package references**

Add to the csproj:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\RustMapsApi.Assets\RustMapsApi.Assets.csproj" />
  </ItemGroup>
```

`Microsoft.Extensions.DependencyInjection` is already versioned centrally, so no
`Directory.Packages.props` edit is needed. (Confirm with
`dotnet add samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj package Microsoft.Extensions.DependencyInjection`
only if the reference fails to resolve.)

- [ ] **Step 2: Create the tile view model**

`samples/RustMapsApi.Assets.GalleryApp/ViewModels/MonumentIconViewModel.cs`:

```csharp
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

public sealed class MonumentIconViewModel
{
    public MonumentIconViewModel(MonumentAsset asset)
    {
        TypeName = asset.MonumentType.ToString();
        AssetName = asset.AssetName;
    }

    public string TypeName { get; }

    public string AssetName { get; }
}
```

- [ ] **Step 3: Create the window view model**

`samples/RustMapsApi.Assets.GalleryApp/ViewModels/MainWindowViewModel.cs`:

```csharp
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

public sealed class MainWindowViewModel
{
    public MainWindowViewModel(IMonumentAssetSource assets)
    {
        Icons = assets.AvailableTypes
            .Select(assets.GetAsset)
            .OrderBy(asset => asset.AssetName, StringComparer.Ordinal)
            .Select(asset => new MonumentIconViewModel(asset))
            .ToList();

        Header = $"{Icons.Count} monument icons";
    }

    public IReadOnlyList<MonumentIconViewModel> Icons { get; }

    public string Header { get; }
}
```

- [ ] **Step 4: Build the provider and inject the view model**

Replace the body of `OnFrameworkInitializationCompleted` in `App.axaml.cs` and add the two `using`s:

```csharp
using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.Assets.GalleryApp.ViewModels;
```

```csharp
    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection()
            .AddRustMapsAssets()
            .AddSingleton<MainWindowViewModel>()
            .BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
```

- [ ] **Step 5: Bind the list in the window**

Replace `MainWindow.axaml` with the bound layout (names only for now):

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:RustMapsApi.Assets.GalleryApp.ViewModels"
        x:Class="RustMapsApi.Assets.GalleryApp.MainWindow"
        x:DataType="vm:MainWindowViewModel"
        Width="900" Height="640"
        Title="RustMaps Monument Icons">
  <DockPanel Margin="16">
    <TextBlock DockPanel.Dock="Top"
               Text="{Binding Header}"
               FontSize="20" FontWeight="SemiBold"
               Margin="0,0,0,12" />
    <ScrollViewer>
      <ItemsControl ItemsSource="{Binding Icons}">
        <ItemsControl.ItemsPanel>
          <ItemsPanelTemplate>
            <WrapPanel />
          </ItemsPanelTemplate>
        </ItemsControl.ItemsPanel>
        <ItemsControl.ItemTemplate>
          <DataTemplate x:DataType="vm:MonumentIconViewModel">
            <Border Width="140" Margin="8" Padding="8"
                    BorderThickness="1" BorderBrush="#22808080" CornerRadius="6">
              <TextBlock Text="{Binding TypeName}"
                         HorizontalAlignment="Center"
                         TextWrapping="Wrap" />
            </Border>
          </DataTemplate>
        </ItemsControl.ItemTemplate>
      </ItemsControl>
    </ScrollViewer>
  </DockPanel>
</Window>
```

- [ ] **Step 6: Build and verify**

Run:

```bash
dotnet build RustMapsApi.slnx -c Release
```

Expected: build succeeds. On a machine with a display, launching shows a header
"N monument icons" and a wrapped grid of monument type names. (Headless: build is the gate.)

- [ ] **Step 7: Format and commit**

```bash
dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"
git add samples/RustMapsApi.Assets.GalleryApp Directory.Packages.props
git commit -m "feat(samples): wire AddRustMapsAssets DI + monument list"
```

---

### Task 3: Render the SVG icons

Add `Avalonia.Svg.Skia`, convert each asset's SVG to a bound image, and show it in each tile.

**Files:**

- Modify: `samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj` (add package)
- Create: `samples/RustMapsApi.Assets.GalleryApp/SvgImageFactory.cs`
- Modify: `samples/RustMapsApi.Assets.GalleryApp/ViewModels/MonumentIconViewModel.cs` (add `Image`)
- Modify: `samples/RustMapsApi.Assets.GalleryApp/ViewModels/MainWindowViewModel.cs` (pass the image)
- Modify: `samples/RustMapsApi.Assets.GalleryApp/MainWindow.axaml` (add `<Image>`)
- Modify: `Directory.Packages.props` (Avalonia.Svg.Skia — via `dotnet add`)

**Interfaces:**

- Consumes: `RustMapsApi.V4.Assets.MonumentAsset.OpenStream() : Stream`;
  `Avalonia.Svg.Skia.SvgSource.LoadFromSvgDocument(Svg.SvgDocument)`; `Avalonia.Svg.Skia.SvgImage`
  (implements `Avalonia.Media.IImage`); `Svg.SvgDocument.Open<SvgDocument>(Stream)`.
- Produces: `SvgImageFactory.Create(MonumentAsset) : SvgImage` and a new
  `MonumentIconViewModel.Image` of type `Avalonia.Media.IImage`.

- [ ] **Step 1: Add the rendering package**

```bash
dotnet add samples/RustMapsApi.Assets.GalleryApp/RustMapsApi.Assets.GalleryApp.csproj package Avalonia.Svg.Skia
```

Offline fallback — add to `Directory.Packages.props` (match the Avalonia line):

```xml
<PackageVersion Include="Avalonia.Svg.Skia" Version="11.3.0" />
```

and add `<PackageReference Include="Avalonia.Svg.Skia" />` to the csproj's Avalonia `ItemGroup`.

- [ ] **Step 2: Create the SVG→image factory (the single Svg.Skia isolation point)**

`samples/RustMapsApi.Assets.GalleryApp/SvgImageFactory.cs`:

```csharp
using Avalonia.Svg.Skia;
using RustMapsApi.V4.Assets;
using Svg;

namespace RustMapsApi.Assets.GalleryApp;

// The only place that touches Avalonia.Svg.Skia. It reads the SVG from the *package*
// (MonumentAsset.OpenStream) so the sample proves the package delivers the icon. If the
// pinned Avalonia.Svg.Skia version exposes a different loader, adjust ONLY this method —
// everything else binds to Avalonia.Media.IImage and is unaffected.
internal static class SvgImageFactory
{
    public static SvgImage Create(MonumentAsset asset)
    {
        using var stream = asset.OpenStream();
        var document = SvgDocument.Open<SvgDocument>(stream);
        return new SvgImage { Source = SvgSource.LoadFromSvgDocument(document) };
    }
}
```

- [ ] **Step 3: Add the image to the tile view model**

Edit `MonumentIconViewModel.cs` to accept and expose the image:

```csharp
using Avalonia.Media;
using RustMapsApi.V4.Assets;

namespace RustMapsApi.Assets.GalleryApp.ViewModels;

public sealed class MonumentIconViewModel
{
    public MonumentIconViewModel(MonumentAsset asset, IImage image)
    {
        TypeName = asset.MonumentType.ToString();
        AssetName = asset.AssetName;
        Image = image;
    }

    public string TypeName { get; }

    public string AssetName { get; }

    public IImage Image { get; }
}
```

- [ ] **Step 4: Pass the rendered image from the window view model**

In `MainWindowViewModel.cs`, change the projection to build the image:

```csharp
            .Select(asset => new MonumentIconViewModel(asset, SvgImageFactory.Create(asset)))
```

(The full `Icons` initializer becomes:)

```csharp
        Icons = assets.AvailableTypes
            .Select(assets.GetAsset)
            .OrderBy(asset => asset.AssetName, StringComparer.Ordinal)
            .Select(asset => new MonumentIconViewModel(asset, SvgImageFactory.Create(asset)))
            .ToList();
```

- [ ] **Step 5: Show the image in the tile**

In `MainWindow.axaml`, replace the tile's `<TextBlock>` with an image above the name. The `<Border>`
content becomes:

```xml
            <Border Width="140" Margin="8" Padding="8"
                    BorderThickness="1" BorderBrush="#22808080" CornerRadius="6">
              <StackPanel HorizontalAlignment="Center">
                <Image Source="{Binding Image}" Width="96" Height="96" />
                <TextBlock Text="{Binding TypeName}"
                           HorizontalAlignment="Center"
                           TextWrapping="Wrap"
                           Margin="0,8,0,0" />
              </StackPanel>
            </Border>
```

- [ ] **Step 6: Build and verify rendering**

Run:

```bash
dotnet build RustMapsApi.slnx -c Release
```

Expected: build succeeds. On a machine with a display, launching shows a grid of **rendered**
monument icons, each labelled. If the build fails on the `Svg.SvgDocument` type not resolving, add an
explicit `<PackageReference Include="Svg.Skia" />` (versioned centrally) — it provides the `Svg`
DOM that `Avalonia.Svg.Skia` parses with. Headless: build is the gate; note that live render
verification is deferred to a display/CI.

- [ ] **Step 7: Format and commit**

```bash
dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"
git add samples/RustMapsApi.Assets.GalleryApp Directory.Packages.props
git commit -m "feat(samples): render monument icons with Avalonia.Svg.Skia"
```

---

### Task 4: Gallery sample README

**Files:**

- Create: `samples/RustMapsApi.Assets.GalleryApp/README.md`

- [ ] **Step 1: Write the sample README**

`samples/RustMapsApi.Assets.GalleryApp/README.md`:

```markdown
# Monument icon gallery (Avalonia)

A cross-platform desktop app that renders **every** monument icon bundled in
[`RustMapsApi.Assets`](../../src/RustMapsApi.Assets/README.md) — no API key, no network.

It builds a `ServiceCollection`, calls `AddRustMapsAssets()`, injects `IMonumentAssetSource`,
enumerates `AvailableTypes`, and renders each icon (SVG) with
[Avalonia.Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia).

## Run

```bash
dotnet run --project samples/RustMapsApi.Assets.GalleryApp
```

A window opens with a scrollable grid of monument icons, each labelled with its `MonumentType`.

<!-- Optional: drop a screenshot.png beside this file and reference it here. -->
```

- [ ] **Step 2: Commit**

```bash
git add samples/RustMapsApi.Assets.GalleryApp/README.md
git commit -m "docs(samples): add gallery app README"
```

---

### Task 5: Root README — Packages, Install, Monument icons

**Files:**

- Modify: `README.md`

**Interfaces:**

- Consumes: the sample path `samples/RustMapsApi.Assets.GalleryApp` from Task 1.

- [ ] **Step 1: Add a Packages section and update Install**

In `README.md`, replace the `## Install` section:

```markdown
## Install

```bash
dotnet add package RustMapsApi
```
```

with:

```markdown
## Packages

| Package | NuGet | Description |
| --- | --- | --- |
| **RustMapsApi** | [![NuGet](https://img.shields.io/nuget/v/RustMapsApi.svg)](https://www.nuget.org/packages/RustMapsApi) | The v4 API client — `IRustMapsClient`, `Result<T>`, models & requests, DI. |
| **RustMapsApi.Assets** | [![NuGet](https://img.shields.io/nuget/v/RustMapsApi.Assets.svg)](https://www.nuget.org/packages/RustMapsApi.Assets) | Monument icon artwork (SVG) mapped by `MonumentType` — resolve a monument's icon offline. |

## Install

```bash
dotnet add package RustMapsApi          # the API client
dotnet add package RustMapsApi.Assets   # optional monument icons
```
```

- [ ] **Step 2: Add a "Monument icons" section**

In `README.md`, immediately before `## Build and test`, insert:

```markdown
## Monument icons

[`RustMapsApi.Assets`](src/RustMapsApi.Assets/README.md) bundles RustMaps' monument icon artwork
(SVG) and maps each `MonumentType` to its icon — resolve a monument's icon straight from an API
response with no network call:

```csharp
using RustMapsApi.V4.Assets;

foreach (var monument in map.Monuments ?? [])
{
    if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
        File.WriteAllText(asset.FileName, asset.GetSvg());
}
```

See the runnable [Avalonia icon gallery](samples/RustMapsApi.Assets.GalleryApp/README.md) that
renders every bundled icon.
```

- [ ] **Step 3: Verify and commit**

Check the Markdown renders (no `MD036`/`MD040` issues; the code fences all have languages) and links
resolve. Then:

```bash
git add README.md
git commit -m "docs: document RustMapsApi.Assets in the root README"
```

---

### Task 6: DocFX landing page + introduction

**Files:**

- Modify: `docs/index.md`
- Modify: `docs/articles/introduction.md`

- [ ] **Step 1: Add the Assets row to the landing Packages table**

In `docs/index.md`, under `## Packages`, add a second table row after the `RustMapsApi` row:

```markdown
| **[RustMapsApi.Assets](articles/assets.md)** | [![Downloads](https://img.shields.io/nuget/dt/RustMapsApi.Assets.svg)](https://www.nuget.org/packages/RustMapsApi.Assets) | Monument icon artwork (SVG) mapped by `MonumentType` — resolve a monument's icon offline, no network call. |
```

- [ ] **Step 2: Add a feature card**

In `docs/index.md`, inside the `<div class="rp-grid">` block, add a card (after the DI card):

```html
  <a class="rp-card" href="articles/assets.md">
    <div class="rp-card-icon">🖼️</div>
    <h3>Monument icons</h3>
    <p>Resolve a monument's icon (SVG) straight from a <code>MonumentType</code> — offline, no network call.</p>
  </a>
```

- [ ] **Step 3: Turn "One package" into a two-row Packages table**

In `docs/articles/introduction.md`, replace the `## One package` heading and its single-row table
with:

```markdown
## Packages

| Package | Description |
| --- | --- |
| [`RustMapsApi`](xref:RustMapsApi) | The V4 client, `Result<T>`, models, requests, and DI registration. |
| [`RustMapsApi.Assets`](assets.md) | Monument icon artwork (SVG) mapped by `MonumentType` — resolve a monument's icon offline. |
```

- [ ] **Step 4: Add a "What you can do" bullet and a next-step link**

In `docs/articles/introduction.md`, add to the `## What you can do` list:

```markdown
- **Render monument icons** — resolve a monument's icon (SVG) straight from its `MonumentType`,
  offline, with the companion [`RustMapsApi.Assets`](assets.md) package.
```

and add to `## Next steps`:

```markdown
- Need icons? See the [Assets](assets.md) guide.
```

- [ ] **Step 5: Commit**

```bash
git add docs/index.md docs/articles/introduction.md
git commit -m "docs: surface RustMapsApi.Assets on the docs landing + introduction"
```

---

### Task 7: New Assets guide article + TOC entry

**Files:**

- Create: `docs/articles/assets.md`
- Modify: `docs/articles/toc.yml`

- [ ] **Step 1: Write the Assets guide**

`docs/articles/assets.md`:

```markdown
# Assets

[`RustMapsApi.Assets`](https://www.nuget.org/packages/RustMapsApi.Assets) bundles RustMaps' monument
icon artwork (SVG) and maps each [`MonumentType`](xref:RustMapsApi.V4.Models.MonumentType) to its
icon. Given a monument from an API response, get its icon with **no network call**.

## Install

```bash
dotnet add package RustMapsApi.Assets
```

## Resolve an icon

[`MonumentAssets`](xref:RustMapsApi.V4.Assets.MonumentAssets) is the static entry point. Prefer
`TryGetAsset` — it never throws:

```csharp
using RustMapsApi.V4.Assets;

if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
{
    string svg      = asset.GetSvg();      // markup
    byte[] bytes    = asset.GetBytes();    // raw bytes
    using Stream s  = asset.OpenStream();  // caller disposes
}
```

| Member | Purpose |
| --- | --- |
| `TryGetAsset(type, out asset)` | `true` + the asset when an icon exists; otherwise `false`. |
| `GetAsset(type)` | The asset, or throws `KeyNotFoundException` when none exists. |
| `HasAsset(type)` | Whether an icon exists for the type. |
| `AvailableTypes` | Every `MonumentType` that has an icon. |

## The `MonumentAsset`

A resolved [`MonumentAsset`](xref:RustMapsApi.V4.Assets.MonumentAsset) describes one icon:

| Member | Example |
| --- | --- |
| `MonumentType` | `MonumentType.LaunchSite` |
| `AssetName` | `"Launch_Site"` |
| `FileName` | `"Launch_Site.svg"` |
| `MediaType` | `"image/svg+xml"` |
| `SourceUri` | `https://content.rustmaps.com/assets/Launch_Site.svg` |
| `OpenStream()` | A readable stream over the embedded SVG (you dispose it). |
| `GetBytes()` | The SVG as `byte[]`. |
| `GetSvg()` | The SVG markup as a `string`. |

## Coverage

Not every `MonumentType` has an icon. Terrain types (`Mountain*`, `Lake*`) and the `Unknown` /
`NotImplemented` / `CustomMonument` sentinels return `false` from `TryGetAsset` (and `GetAsset`
throws for them). Use `HasAsset` or enumerate `AvailableTypes`:

```csharp
foreach (var type in MonumentAssets.AvailableTypes)
    Console.WriteLine($"{type} -> {MonumentAssets.GetAsset(type).FileName}");
```

## From an API response

Every `MonumentType` comes straight off a map's monuments, so an icon is one lookup away:

```csharp
var result = await client.GetMapByIdAsync(mapId);
foreach (var monument in result.Data?.Monuments ?? [])
{
    if (MonumentAssets.TryGetAsset(monument.Type, out var asset))
        File.WriteAllBytes(asset.FileName, asset.GetBytes());
}
```

## Dependency injection

Register [`IMonumentAssetSource`](xref:RustMapsApi.V4.Assets.IMonumentAssetSource) — the same
surface as the static class, injectable — with `AddRustMapsAssets`:

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddRustMapsAssets();  // registers IMonumentAssetSource (singleton)
```

```csharp
public sealed class IconService(IMonumentAssetSource assets)
{
    public string? Svg(MonumentType type) =>
        assets.TryGetAsset(type, out var asset) ? asset.GetSvg() : null;
}
```

## Rendering SVG

The bundled art is SVG; neither WPF nor Avalonia renders SVG natively, so feed the asset to an SVG
renderer.

### Avalonia

The runnable [icon gallery sample](https://github.com/HandyS11/RustMapsApi/tree/develop/samples/RustMapsApi.Assets.GalleryApp)
renders every icon with [Avalonia.Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia):

```csharp
using Avalonia.Svg.Skia;
using Svg;

using var stream = MonumentAssets.GetAsset(type).OpenStream();
var document = SvgDocument.Open<SvgDocument>(stream);
var image = new SvgImage { Source = SvgSource.LoadFromSvgDocument(document) };
// bind image to <Image Source="..." />
```

### WPF

Feed `OpenStream()` to a renderer such as
[SharpVectors](https://www.nuget.org/packages/SharpVectors.Wpf):

```csharp
using var stream = MonumentAssets.GetAsset(type).OpenStream();
var reader = new FileSvgReader(new WpfDrawingSettings());
var image = new DrawingImage(reader.Read(stream)); // bind to <Image Source="..." />
```

## Attribution

The monument icon artwork is © [RustMaps](https://rustmaps.com), served from their public CDN
(`content.rustmaps.com`) and redistributed unmodified for convenience. All rights to the artwork
remain with RustMaps; the package code is MIT-licensed.
```

- [ ] **Step 2: Add the guide to the TOC**

In `docs/articles/toc.yml`, add an `Assets` item under `Guides` (after `Client`):

```yaml
- name: Guides
  items:
    - name: Client
      href: client.md
    - name: Assets
      href: assets.md
```

- [ ] **Step 3: Commit**

```bash
git add docs/articles/assets.md docs/articles/toc.yml
git commit -m "docs: add Assets guide article"
```

---

### Task 8: Samples pages

**Files:**

- Modify: `docs/articles/samples.md`
- Modify: `samples/README.md`

- [ ] **Step 1: Update the DocFX samples article**

In `docs/articles/samples.md`, change the opening sentence from "Two runnable console apps …" to
mention three samples, and append a new section before `## Menu`:

```markdown
## Monument icon gallery (Avalonia)

A cross-platform desktop app that renders every monument icon bundled in
[`RustMapsApi.Assets`](assets.md) — **no API key, no network**. It wires the package's DI
(`AddRustMapsAssets`) and injects `IMonumentAssetSource`.

```bash
dotnet run --project samples/RustMapsApi.Assets.GalleryApp
```
```

Also change the intro line to:

```markdown
The [`samples/`](https://github.com/HandyS11/RustMapsApi/tree/develop/samples) folder has three
runnable apps: two console apps over the client, and a desktop icon gallery for the Assets package.
```

- [ ] **Step 2: Update the samples folder README**

In `samples/README.md`, change the opening line to describe three samples and add a section
(after the DI console section, before `## Menu`):

```markdown
## Monument icon gallery (Avalonia)

A cross-platform desktop app that renders every monument icon from
[`RustMapsApi.Assets`](../src/RustMapsApi.Assets/README.md) — **no API key, no network**. It wires
`AddRustMapsAssets()` and injects `IMonumentAssetSource`.

```bash
dotnet run --project samples/RustMapsApi.Assets.GalleryApp
```
```

Change the opening line from "Two runnable console apps demonstrating the two ways to consume
`RustMapsApi`." to:

```markdown
Three runnable samples: two console apps demonstrating the two ways to consume `RustMapsApi`, and a
desktop icon gallery for the companion `RustMapsApi.Assets` package.
```

- [ ] **Step 3: Commit**

```bash
git add docs/articles/samples.md samples/README.md
git commit -m "docs(samples): document the Avalonia icon gallery"
```

---

### Task 9: Full verification

**Files:** none (verification only).

- [ ] **Step 1: Build the whole solution (Release, warnings-as-errors)**

```bash
dotnet build RustMapsApi.slnx -c Release
```

Expected: success. (The sample relaxes its own `TreatWarningsAsErrors`; every other project keeps it.)

- [ ] **Step 2: Run the test suite**

```bash
dotnet test RustMapsApi.slnx -c Release
```

Expected: all existing tests pass; no new tests (the sample adds none). Live integration tests skip
without `RUSTMAPS_API_KEY`.

- [ ] **Step 3: Format check (must be a no-op)**

```bash
dotnet jb cleanupcode RustMapsApi.slnx --profile="ReformatAndReorder"
git diff --exit-code
```

Expected: empty diff (everything already formatted by earlier tasks).

- [ ] **Step 4: Build the docs site**

```bash
dotnet docfx docs/docfx.json
```

Expected: the site builds; `assets.md` appears under Guides; the `xref:RustMapsApi.V4.Assets.*`
links resolve (DocFX prints a warning for any unresolved xref — there should be none new). If
`dotnet restore`/DocFX cannot reach NuGet here, report that build verification is deferred to CI
rather than claiming success.

- [ ] **Step 5: Manual render check (deferred if headless)**

On a machine with a display:

```bash
dotnet run --project samples/RustMapsApi.Assets.GalleryApp
```

Expected: a window of rendered monument icons. Capture a screenshot for the sample README if desired.
In a headless environment, state explicitly that the live render is unverified here and covered by
manual/maintainer check.

## Self-Review

**Spec coverage**

- Sample project (offline, DI, Avalonia, Svg.Skia isolated, slnx + CPM) → Tasks 1–4.
- Gate handling (`GenerateDocumentationFile=false`, `TreatWarningsAsErrors=false`, jb format,
  no tests, Sonar/coverage/mutation exclusion) → Global Constraints + Tasks 1–3 + Task 9.
- Root README (Packages, Install, Monument icons) → Task 5.
- `docs/index.md` (Packages row + card) and `introduction.md` (Packages table + bullet) → Task 6.
- New `assets.md` guide + `toc.yml` → Task 7.
- `samples.md` + `samples/README.md` → Task 8.
- Verification (build, test, format, docs build, headless caveat) → Task 9.

**Placeholder scan:** no TBD/TODO; every code and doc block is complete. The sample README's
screenshot is an explicit optional HTML comment, not a required placeholder.

**Type consistency:** `MonumentIconViewModel(MonumentAsset)` in Task 2 is widened to
`MonumentIconViewModel(MonumentAsset, IImage)` in Task 3 (the constructor call site in
`MainWindowViewModel` is updated in the same task). `SvgImageFactory.Create` returns `SvgImage`
(an `IImage`); the view model stores `IImage`. `IMonumentAssetSource.AvailableTypes` /`GetAsset`
match the real interface. `AddRustMapsAssets` lives in the `Microsoft.Extensions.DependencyInjection`
namespace (the Assets package). All consistent.
```
