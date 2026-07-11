# Samples

Three runnable samples: two console apps demonstrating the two ways to consume `RustMapsApi`, and a
desktop icon gallery for the companion `RustMapsApi.Assets` package.
Both present the same interactive menu over the full `IRustMapsClient` surface;
they differ only in how the client is constructed and where the API key comes
from.

> **Read operations** (lookup, search, limits, configs) are safe to run freely.
> **Write operations** (create / upload) **consume real generation credits**
> against your RustMaps account and are gated behind a `yes` confirmation.

## Standalone (`HttpClient`)

Builds the client by hand and reads the key from an environment variable.

```bash
export RUSTMAPS_API_KEY="YOUR_KEY"
dotnet run --project samples/RustMapsApi.Standalone.ConsoleApp
```

## Dependency injection (Generic Host)

Registers the client with `AddRustMapsClientV4(IConfiguration)` and reads the key
from configuration. `appsettings.json` ships a `REPLACE_ME` placeholder; supply
the real key with user-secrets:

```bash
dotnet user-secrets set "RustMaps:ApiKey" "YOUR_KEY" \
  --project samples/RustMapsApi.DependencyInjection.ConsoleApp
dotnet run --project samples/RustMapsApi.DependencyInjection.ConsoleApp
```

Until a real key is supplied, the app detects the `REPLACE_ME` placeholder (and a
blank key via `ValidateOnStart`), prints the command above, and exits non-zero.

## Monument icon gallery (Avalonia)

A cross-platform desktop app that renders every monument icon from
[`RustMapsApi.Assets`](../src/RustMapsApi.Assets/README.md) — **no API key, no network**. It wires
`AddRustMapsAssets()` and injects `IMonumentAssetSource`.

```bash
dotnet run --project samples/RustMapsApi.Assets.GalleryApp
```

## Menu

```text
  1) Get map by id
  2) Get map by seed + size
  3) Get map by URL
  4) Search (by size range)
  5) Search by saved filter
  6) Get generation limits
  7) Get saved configs
  8) Get map settings
  9) Get default custom config
  -- writes (consume credits) --
  10) Create map
  11) Create custom map (from default config)
  12) Create custom map from saved config
  13) Upload map save file
  q) Quit
```

Every call returns `Result<T>`: the samples check `IsSuccess`, then print key
fields from `Data` or the `Error.Kind` / message from `Error`.
