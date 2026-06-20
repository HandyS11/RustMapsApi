---
_disableToc: true
_disableAffix: true
_disableBreadcrumb: true
---

<div class="rp-hero">
  <img class="rp-hero-logo" src="images/fav-icon.png" alt="RustMapsApi logo" />
  <h1>RustMapsApi</h1>
  <p class="rp-tagline">
    A strongly-typed .NET client for the public <a href="https://rustmaps.com">RustMaps</a> API (v4).
    Look up procedural and custom maps, search, request map generation, upload saves, and read your
    generation limits — with <code>Result&lt;T&gt;</code> everywhere and first-class dependency injection.
  </p>
  <div class="rp-cta">
    <a class="btn btn-primary" href="articles/getting-started.md">Get Started</a>
    <a class="btn btn-outline-secondary" href="api/RustMapsApi.yml">API Reference</a>
  </div>
  <p class="rp-badges">
    <img src="https://img.shields.io/badge/.NET-Standard%202.0%20%7C%2010-512BD4" alt=".NET" />
    <img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="MIT" />
    <img src="https://img.shields.io/nuget/v/RustMapsApi.svg?label=NuGet&color=ce412b" alt="NuGet" />
  </p>
</div>

<div class="rp-grid">
  <a class="rp-card" href="articles/client.md">
    <div class="rp-card-icon">🗺️</div>
    <h3>Map lookup</h3>
    <p>Fetch maps by id, seed &amp; size, or RustMaps URL — one typed <code>Result&lt;T&gt;</code> API.</p>
  </a>
  <a class="rp-card" href="articles/client.md">
    <div class="rp-card-icon">🔎</div>
    <h3>Search</h3>
    <p>Structured queries or saved homepage filters, paged with a zero-based index.</p>
  </a>
  <a class="rp-card" href="articles/client.md">
    <div class="rp-card-icon">⚙️</div>
    <h3>Map generation</h3>
    <p>Request procedural or custom maps, generate from a saved config, or upload a save file.</p>
  </a>
  <a class="rp-card" href="articles/client.md">
    <div class="rp-card-icon">📊</div>
    <h3>Limits &amp; configs</h3>
    <p>Read your generation limits and saved/default custom-map configurations.</p>
  </a>
  <a class="rp-card" href="articles/getting-started.md">
    <div class="rp-card-icon">🧩</div>
    <h3>Dependency injection</h3>
    <p>Register with <code>AddRustMapsClientV4</code> and inject <code>IRustMapsClient</code>, or new it up by hand.</p>
  </a>
  <a class="rp-card" href="articles/introduction.md">
    <div class="rp-card-icon">🎯</div>
    <h3>Broad targeting</h3>
    <p>.NET Standard 2.0 and .NET 10 — runs on .NET Framework 4.6.2+, .NET 6–10, Mono and Unity.</p>
  </a>
</div>

## Packages

| Package | Downloads | Description |
| --- | --- | --- |
| **[RustMapsApi](xref:RustMapsApi)** | [![Downloads](https://img.shields.io/nuget/dt/RustMapsApi.svg)](https://www.nuget.org/packages/RustMapsApi) | Strongly-typed RustMaps API v4 client — `IRustMapsClient`, `Result<T>`, models & requests, and DI registration. |

## Quickstart

```csharp
using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.V4;

services.AddRustMapsClientV4(options => options.ApiKey = "YOUR_API_KEY");

// then inject IRustMapsClient
public sealed class MapService(IRustMapsClient client)
{
    public async Task<string?> GetUrlAsync(string mapId)
    {
        var result = await client.GetMapByIdAsync(mapId);
        return result.IsSuccess ? result.Data!.Url : null;
    }
}
```

Don't have an API key yet? The **[Getting Started](articles/getting-started.md)** guide walks you
through getting one and making your first call.
