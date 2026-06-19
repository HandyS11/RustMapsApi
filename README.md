# RustMapsApi

Strongly-typed .NET client for the public [RustMaps API](https://rustmaps.com) (v4),
shipped as a single NuGet package targeting `netstandard2.0` and `net10.0`.

- **Package:** [`RustMapsApi`](src/RustMapsApi) — version-agnostic core plus the `RustMapsApi.V4` surface.
- Every call returns `Result<T>` (no exceptions for expected API failures).
- `IHttpClientFactory`-backed typed client with `System.Text.Json` source generation.

## Install

```bash
dotnet add package RustMapsApi
```

See the [package README](src/RustMapsApi/README.md) for usage.

## Build and test

```bash
dotnet build RustMapsApi.slnx -c Release
dotnet test
```

Live integration tests run only when `RUSTMAPS_API_KEY` is set; otherwise they are skipped.

## License

[MIT](LICENSE)
