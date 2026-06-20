# Client

The [`IRustMapsClient`](xref:RustMapsApi.V4.IRustMapsClient) interface is the whole public surface.
Every method returns a [`Result<T>`](xref:RustMapsApi.Results.Result`1); register the client with
DI or build it standalone.

## Construction

### Dependency injection

[`AddRustMapsClientV4`](xref:Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions)
has two overloads — configure inline, or bind from `IConfiguration`:

```csharp
using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.V4;

// Inline
services.AddRustMapsClientV4(options =>
{
    options.ApiKey = "YOUR_API_KEY";
    // options.BaseAddress and options.Timeout have sensible defaults
});

// From configuration (binds RustMapsClientOptions)
services.AddRustMapsClientV4(configuration.GetSection("RustMaps"));
```

Then inject `IRustMapsClient` anywhere. Configuration is bound to
[`RustMapsClientOptions`](xref:RustMapsApi.RustMapsClientOptions) (`ApiKey`, `BaseAddress`,
`Timeout`).

### Standalone

Build the client with your own `HttpClient`. Set the base address and the `X-API-Key` header
yourself:

```csharp
using var http = new HttpClient { BaseAddress = new Uri("https://api.rustmaps.com") };
http.DefaultRequestHeaders.Add("X-API-Key", "YOUR_API_KEY");
var client = new RustMapsApi.V4.RustMapsClient(http);
```

## The `Result<T>` contract

Every call returns a [`Result<T>`](xref:RustMapsApi.Results.Result`1). Check `IsSuccess`, then read
`Data` (on success) or `Error` (on failure); `StatusCode` carries the HTTP status either way.

```csharp
var result = await client.GetMapByIdAsync(mapId);
if (result.IsSuccess)
    Use(result.Data!);
else
    Console.WriteLine($"{result.Error!.Kind}: {result.Error.Message} ({result.StatusCode})");
```

`Error.Kind` is a [`RustMapsErrorKind`](xref:RustMapsApi.Results.RustMapsErrorKind):

| Kind | HTTP | Meaning |
| --- | --- | --- |
| `NotFound` | 404 | The requested resource was not found. |
| `Unauthorized` | 401 | The API key is missing or invalid. |
| `Forbidden` | 403 | The key lacks access to the resource. |
| `RateLimited` | 429 | The rate limit was exceeded. |
| `Queued` | 409 | The map exists but has not finished generating. |
| `Validation` | 400 | The request body or parameters were invalid. |
| `Transport` | — | A network or transport failure occurred. |
| `Unknown` | — | An unrecognised failure occurred. |

## Reads

| Method | Purpose |
| --- | --- |
| `GetMapByIdAsync(mapId)` | Get a map by its identifier. |
| `GetMapBySeedAndSizeAsync(size, seed, staging)` | Get a procedural map by size and seed. |
| `GetMapByUrlAsync(url)` | Get a map by its RustMaps URL. |
| `SearchAsync(query, page, …)` | Search with a structured query (zero-based page). |
| `SearchByFilterAsync(filterId, page, …)` | Search with a saved homepage filter. |
| `GetLimitsAsync(orgId?)` | Read your current generation limits. |
| `GetSavedConfigsAsync()` | List your saved custom-map configs. |
| `GetMapSettingsAsync(mapId)` | Get the custom settings used to generate a map. |
| `GetDefaultCustomConfigAsync()` | Get the default custom-map configuration. |

## Writes

These **consume generation credits** against your RustMaps account:

| Method | Purpose |
| --- | --- |
| `CreateMapAsync(request)` | Request generation of a standard map. |
| `CreateCustomMapAsync(request)` | Request a custom map from explicit settings. |
| `CreateCustomMapFromConfigAsync(request)` | Request a custom map from a saved config. |
| `UploadMapAsync(upload)` | Upload a pre-generated map save file. |

## Endpoint tiers — what costs what

RustMaps v4 endpoints fall into three tiers. Knowing which is which keeps test runs and
experiments from burning credits or hitting subscription walls:

- **Free GET / query** — no credits, no subscription: `GetLimitsAsync`,
  `GetMapBySeedAndSizeAsync`, `GetMapByIdAsync`, `GetMapByUrlAsync`, `SearchAsync`,
  `SearchByFilterAsync` (needs a filter id created on the RustMaps homepage).
- **Subscription-required GET** — returns 401/403 without a paid plan: `GetMapSettingsAsync`,
  `GetDefaultCustomConfigAsync`, `GetSavedConfigsAsync`.
- **Credit-consuming writes** — spend real generation credits: `CreateMapAsync`,
  `UploadMapAsync`, `CreateCustomMapAsync`, `CreateCustomMapFromConfigAsync`.
