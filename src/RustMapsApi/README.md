# RustMapsApi

Strongly-typed .NET client for the public [RustMaps API](https://rustmaps.com) (v4).
Targets `netstandard2.0` and `net10.0`.

## Install

```bash
dotnet add package RustMapsApi
```

## Usage (dependency injection)

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

## Usage (standalone)

```csharp
using var http = new HttpClient { BaseAddress = new Uri("https://api.rustmaps.com") };
http.DefaultRequestHeaders.Add("X-API-Key", "YOUR_API_KEY");
var client = new RustMapsApi.V4.RustMapsClient(http);
```

Every call returns `Result<T>` — check `IsSuccess`, then read `Data` or `Error`.
