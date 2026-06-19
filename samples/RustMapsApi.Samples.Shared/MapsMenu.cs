using RustMapsApi.V4;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Samples.Shared;

/// <summary>An interactive console menu over <see cref="IRustMapsClient"/>.</summary>
public static class MapsMenu
{
    /// <summary>Runs the menu loop until the user quits.</summary>
    public static async Task RunAsync(IRustMapsClient client, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(client);

        while (!ct.IsCancellationRequested)
        {
            PrintMenu();
            var choice = ConsolePrompt.ReadLine("Choice: ");
            Console.WriteLine();

            switch (choice)
            {
                case "1": await GetMapByIdAsync(client, ct); break;
                case "2": await GetMapBySeedAndSizeAsync(client, ct); break;
                case "3": await GetMapByUrlAsync(client, ct); break;
                case "4": await SearchAsync(client, ct); break;
                case "5": await SearchByFilterAsync(client, ct); break;
                case "6": await GetLimitsAsync(client, ct); break;
                case "7": await GetSavedConfigsAsync(client, ct); break;
                case "8": await GetMapSettingsAsync(client, ct); break;
                case "9": await GetDefaultCustomConfigAsync(client, ct); break;
                case "10": await CreateMapAsync(client, ct); break;
                case "11": await CreateCustomMapAsync(client, ct); break;
                case "12": await CreateCustomMapFromConfigAsync(client, ct); break;
                case "13": await UploadMapAsync(client, ct); break;
                case "q" or "Q": return;
                default: Console.WriteLine("Unknown choice."); break;
            }

            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("=== RustMaps sample ===");
        Console.WriteLine("  1) Get map by id");
        Console.WriteLine("  2) Get map by seed + size");
        Console.WriteLine("  3) Get map by URL");
        Console.WriteLine("  4) Search (by size range)");
        Console.WriteLine("  5) Search by saved filter");
        Console.WriteLine("  6) Get generation limits");
        Console.WriteLine("  7) Get saved configs");
        Console.WriteLine("  8) Get map settings");
        Console.WriteLine("  9) Get default custom config");
        Console.WriteLine("  -- writes (consume credits) --");
        Console.WriteLine("  10) Create map");
        Console.WriteLine("  11) Create custom map (from default config)");
        Console.WriteLine("  12) Create custom map from saved config");
        Console.WriteLine("  13) Upload map save file");
        Console.WriteLine("  q) Quit");
    }

    private static async Task GetMapByIdAsync(IRustMapsClient client, CancellationToken ct)
    {
        var id = ConsolePrompt.ReadLine("Map id: ");
        ResultRenderer.Render(await client.GetMapByIdAsync(id, ct));
    }

    private static async Task GetMapBySeedAndSizeAsync(IRustMapsClient client, CancellationToken ct)
    {
        var size = ConsolePrompt.ReadInt("Size", 4500);
        var seed = ConsolePrompt.ReadInt("Seed", 1234);
        ResultRenderer.Render(await client.GetMapBySeedAndSizeAsync(size, seed, staging: false, ct));
    }

    private static async Task GetMapByUrlAsync(IRustMapsClient client, CancellationToken ct)
    {
        var url = ConsolePrompt.ReadLine("Map URL: ");
        ResultRenderer.Render(await client.GetMapByUrlAsync(url, ct));
    }

    private static async Task SearchAsync(IRustMapsClient client, CancellationToken ct)
    {
        var min = ConsolePrompt.ReadInt("Min size", 3000);
        var max = ConsolePrompt.ReadInt("Max size", 4500);
        var page = ConsolePrompt.ReadInt("Page (zero-based)", 0);
        // SearchQuery exposes many filters; this sample uses the size range only.
        var query = new SearchQuery { Size = new MinMaxFilter(min, max) };
        ResultRenderer.Render(await client.SearchAsync(query, page, options: null, orgId: null, ct));
    }

    private static async Task SearchByFilterAsync(IRustMapsClient client, CancellationToken ct)
    {
        var filterId = ConsolePrompt.ReadLine("Filter id: ");
        var page = ConsolePrompt.ReadInt("Page (zero-based)", 0);
        ResultRenderer.Render(await client.SearchByFilterAsync(filterId, page, options: null, orgId: null, ct));
    }

    private static async Task GetLimitsAsync(IRustMapsClient client, CancellationToken ct)
    {
        ResultRenderer.Render(await client.GetLimitsAsync(orgId: null, ct));
    }

    private static async Task GetSavedConfigsAsync(IRustMapsClient client, CancellationToken ct)
    {
        ResultRenderer.Render(await client.GetSavedConfigsAsync(ct));
    }

    private static async Task GetMapSettingsAsync(IRustMapsClient client, CancellationToken ct)
    {
        var id = ConsolePrompt.ReadLine("Map id: ");
        ResultRenderer.Render(await client.GetMapSettingsAsync(id, ct));
    }

    private static async Task GetDefaultCustomConfigAsync(IRustMapsClient client, CancellationToken ct)
    {
        ResultRenderer.Render(await client.GetDefaultCustomConfigAsync(ct));
    }

    private static async Task CreateMapAsync(IRustMapsClient client, CancellationToken ct)
    {
        var size = ConsolePrompt.ReadInt("Size", 4500);
        var seed = ConsolePrompt.ReadInt("Seed", 1234);
        if (!ConsolePrompt.Confirm($"This will generate a {size}/{seed} map and CONSUME CREDITS."))
        {
            Console.WriteLine("  Cancelled.");
            return;
        }

        var request = new MapGenerationRequest { Size = size, Seed = seed, Staging = false };
        ResultRenderer.Render(await client.CreateMapAsync(request, ct));
    }

    private static async Task CreateCustomMapAsync(IRustMapsClient client, CancellationToken ct)
    {
        var size = ConsolePrompt.ReadInt("Size", 4500);
        var seed = ConsolePrompt.ReadInt("Seed", 1234);
        if (!ConsolePrompt.Confirm("This will generate a custom map and CONSUME CREDITS."))
        {
            Console.WriteLine("  Cancelled.");
            return;
        }

        // Fetch the default custom-map settings, then submit them unchanged.
        var defaults = await client.GetDefaultCustomConfigAsync(ct);
        if (!defaults.IsSuccess)
        {
            Console.WriteLine("  Could not fetch default custom config:");
            ResultRenderer.RenderFailure(defaults);
            return;
        }

        var request = new CreateCustomMapRequest
        {
            MapParameters = new MapGenerationRequest { Size = size, Seed = seed, Staging = false },
            CustomMapSettings = defaults.Data!,
        };
        ResultRenderer.Render(await client.CreateCustomMapAsync(request, ct));
    }

    private static async Task CreateCustomMapFromConfigAsync(IRustMapsClient client, CancellationToken ct)
    {
        var size = ConsolePrompt.ReadInt("Size", 4500);
        var seed = ConsolePrompt.ReadInt("Seed", 1234);
        var configName = ConsolePrompt.ReadLine("Saved config name: ");
        if (!ConsolePrompt.Confirm($"This will generate a custom map from '{configName}' and CONSUME CREDITS."))
        {
            Console.WriteLine("  Cancelled.");
            return;
        }

        var request = new CreateCustomMapFromConfigRequest
        {
            MapParameters = new MapGenerationRequest { Size = size, Seed = seed, Staging = false },
            ConfigName = configName,
        };
        ResultRenderer.Render(await client.CreateCustomMapFromConfigAsync(request, ct));
    }

    private static async Task UploadMapAsync(IRustMapsClient client, CancellationToken ct)
    {
        var path = ConsolePrompt.ReadLine("Path to .map save file: ");
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
        {
            Console.WriteLine("  File not found; cancelled.");
            return;
        }

        if (!ConsolePrompt.Confirm("This will upload the map and CONSUME CREDITS."))
        {
            Console.WriteLine("  Cancelled.");
            return;
        }

        await using var stream = File.OpenRead(path);
        var upload = new MapUpload
        {
            Map = stream,
            FileName = Path.GetFileName(path),
            Staging = false,
        };
        ResultRenderer.Render(await client.UploadMapAsync(upload, ct));
    }
}
