# Introduction

**RustMapsApi** is a strongly-typed .NET client for the public
[RustMaps](https://rustmaps.com) API (version 4). It wraps the v4 HTTP endpoints behind a single
[`IRustMapsClient`](xref:RustMapsApi.V4.IRustMapsClient) interface, returns a typed
[`Result<T>`](xref:RustMapsApi.Results.Result`1) from every call, and registers cleanly with
`Microsoft.Extensions.DependencyInjection`.

## What you can do

- **Look up maps** — by id, by seed &amp; size, or by RustMaps URL.
- **Search** — with a structured query or a saved homepage filter, paged with a zero-based index.
- **Generate maps** — request procedural or custom maps, generate from a saved config, or upload a
  pre-generated save file.
- **Read limits &amp; configs** — your current generation limits and your saved/default custom-map
  configurations.
- **Render monument icons** — resolve a monument's icon (SVG) straight from its `MonumentType`,
  offline, with the companion [`RustMapsApi.Assets`](assets.md) package.

## Packages

| Package | Description |
| --- | --- |
| [`RustMapsApi`](xref:RustMapsApi) | The V4 client, `Result<T>`, models, requests, and DI registration. |
| [`RustMapsApi.Assets`](assets.md) | Monument icon artwork (SVG) mapped by `MonumentType` — resolve a monument's icon offline. |

## Supported runtimes

RustMapsApi targets **.NET Standard 2.0** and **.NET 10**, so it runs on .NET Framework 4.6.2+,
.NET 6 through 10, Mono, and Unity.

## Next steps

- New here? Head to [Getting Started](getting-started.md).
- Want the full surface? See the [Client](client.md) guide.
- Need icons? See the [Assets](assets.md) guide.
- Prefer to read code? Browse the [Samples](samples.md).
