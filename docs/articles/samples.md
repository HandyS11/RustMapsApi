# Samples

Two runnable console apps in [`samples/`](https://github.com/HandyS11/RustMapsApi/tree/develop/samples)
demonstrate the two ways to consume RustMapsApi. Both present the same interactive menu over the
full `IRustMapsClient` surface; they differ only in how the client is constructed and where the API
key comes from.

> **Reads** (lookup, search, limits, configs) are safe to run freely. **Writes** (create / upload)
> **consume real generation credits** and are gated behind a `yes` confirmation.

## Standalone (`HttpClient`)

Builds the client by hand and reads the key from an environment variable.

```bash
export RUSTMAPS_API_KEY="YOUR_KEY"
dotnet run --project samples/RustMapsApi.Standalone.ConsoleApp
```

## Dependency injection (Generic Host)

Registers the client with `AddRustMapsClientV4(IConfiguration)` and reads the key from
configuration. `appsettings.json` ships a `REPLACE_ME` placeholder; supply the real key with
user-secrets:

```bash
dotnet user-secrets set "RustMaps:ApiKey" "YOUR_KEY" \
  --project samples/RustMapsApi.DependencyInjection.ConsoleApp
dotnet run --project samples/RustMapsApi.DependencyInjection.ConsoleApp
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

Every call returns `Result<T>`: the samples check `IsSuccess`, then print key fields from `Data`
or the `Error.Kind` / message from `Error`.
