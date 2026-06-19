using RustMapsApi.V4;
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
}
