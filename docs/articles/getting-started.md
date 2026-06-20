# Getting Started

## 1. Get a RustMaps API key

Sign in at [rustmaps.com](https://rustmaps.com) and copy your API key from your account settings.
The key is sent on every request in the `X-API-Key` header — RustMapsApi handles that for you.

## 2. Install the package

```bash
dotnet add package RustMapsApi
```

## 3. Register the client (dependency injection)

```csharp
using Microsoft.Extensions.DependencyInjection;
using RustMapsApi.V4;

services.AddRustMapsClientV4(options => options.ApiKey = "YOUR_API_KEY");
```

Or bind from configuration (e.g. `appsettings.json` / user-secrets under a `RustMaps` section):

```csharp
services.AddRustMapsClientV4(configuration.GetSection("RustMaps"));
```

Prefer no container? Build the client by hand — see [Client → Construction](client.md#construction).

## 4. Make your first call

Inject [`IRustMapsClient`](xref:RustMapsApi.V4.IRustMapsClient) and always check `IsSuccess`
before reading `Data`:

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

A call that needs no credits and no subscription is a good first test — read your generation
limits:

```csharp
var limits = await client.GetLimitsAsync();
if (limits.IsSuccess)
{
    var concurrent = limits.Data!.Concurrent;
    Console.WriteLine($"Concurrent: {concurrent?.Current}/{concurrent?.Allowed}");
}
```

## Next steps

- The full method surface and the `Result<T>` contract: [Client](client.md).
- Runnable end-to-end examples: [Samples](samples.md).
