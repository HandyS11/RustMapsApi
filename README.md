<div align="center">

# RustMapsApi

**A strongly-typed .NET client for the public [RustMaps API](https://rustmaps.com) (v4).**
Look up procedural and custom maps, request map generation, upload saves, and read your
generation limits — with `Result<T>` everywhere and first-class dependency injection.

[![CI](https://github.com/HandyS11/RustMapsApi/actions/workflows/CI.yml/badge.svg)](https://github.com/HandyS11/RustMapsApi/actions/workflows/CI.yml)
[![CD](https://github.com/HandyS11/RustMapsApi/actions/workflows/CD.yml/badge.svg)](https://github.com/HandyS11/RustMapsApi/actions/workflows/CD.yml)
[![Docs](https://github.com/HandyS11/RustMapsApi/actions/workflows/Documentation.yml/badge.svg)](https://handys11.github.io/RustMapsApi/)

![.NET](https://img.shields.io/badge/.NET-Standard%202.0%20%7C%2010-512BD4)
[![NuGet Version](https://img.shields.io/nuget/v/RustMapsApi.svg)](https://www.nuget.org/packages/RustMapsApi)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![codecov](https://codecov.io/gh/HandyS11/RustMapsApi/branch/develop/graph/badge.svg?token=ua3M5MyCNz)](https://codecov.io/gh/HandyS11/RustMapsApi)
[![Mutation Score](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FHandyS11%2FRustMapsApi%2Fdevelop)](https://dashboard.stryker-mutator.io/reports/github.com/HandyS11/RustMapsApi/develop)

[Getting Started](https://handys11.github.io/RustMapsApi/articles/getting-started.html) ·
[Documentation](https://handys11.github.io/RustMapsApi/) ·
[Samples](samples/README.md)

</div>

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

## Quickstart

**1. Register the client** (dependency injection). Provide your RustMaps API key:

```csharp
using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.V4;

services.AddRustMapsClientV4(options => options.ApiKey = "YOUR_API_KEY");
```

**2. Look up a map.** Inject `IRustMapsClient` and check `IsSuccess` before reading `Data`:

```csharp
public sealed class MapService(IRustMapsClient client)
{
    public async Task<string?> GetUrlAsync(string mapId)
    {
        var result = await client.GetMapByIdAsync(mapId);
        return result.IsSuccess ? result.Data!.Url : null;
    }
}
```

**3. Search, generate, or upload:**

```csharp
// Structured search (zero-based page index)
var page = await client.SearchAsync(query, page: 0);

// Request generation of a procedural map
var status = await client.CreateMapAsync(request);

// Read your current generation limits
var limits = await client.GetLimitsAsync();
```

You can also new up the client directly with your own `HttpClient` — see the
[package README](src/RustMapsApi/README.md) for the standalone usage and the full API surface.

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

## Build and test

```bash
dotnet build RustMapsApi.slnx -c Release
dotnet test
```

Live integration tests run only when `RUSTMAPS_API_KEY` is set; otherwise they are skipped.

## Documentation

Full guides and the API reference live on the
**[documentation site](https://handys11.github.io/RustMapsApi/)** (built with DocFX). Start with
[Getting Started](https://handys11.github.io/RustMapsApi/articles/getting-started.html), then see
the [Client](https://handys11.github.io/RustMapsApi/articles/client.html) guide for the full API
surface. Runnable examples are in [`samples/`](samples/README.md).

## Credits

- Author: [**HandyS11**](https://github.com/HandyS11)
