using RustMapsApi.Results;
using RustMapsApi.V4.Models;
using RustMapsApi.V4.Requests;

namespace RustMapsApi.Samples.Shared;

/// <summary>Renders <see cref="Result{T}"/> outcomes to the console.</summary>
public static class ResultRenderer
{
    /// <summary>Prints the status code, error kind and message for a failed call.</summary>
    public static void RenderFailure<T>(Result<T> result)
    {
        var error = result.Error;
        Console.WriteLine(
            $"  FAILED (HTTP {result.StatusCode}) {error?.Kind}: {error?.Message ?? "(no message)"}");
    }

    /// <summary>Renders a single map.</summary>
    public static void Render(Result<MapInfo> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        var map = result.Data!;
        Console.WriteLine($"  Id={map.Id} Type={map.Type} Size={map.Size} Seed={map.Seed}");
        Console.WriteLine($"  Url={map.Url}");
        Console.WriteLine($"  Custom={map.IsCustomMap} Staging={map.IsStaging} Monuments={map.TotalMonuments}");
    }

    /// <summary>Renders a page of search results.</summary>
    public static void Render(Result<IReadOnlyList<MapThumbnail>> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        var maps = result.Data!;
        Console.WriteLine($"  {maps.Count} result(s):");
        foreach (var map in maps)
        {
            Console.WriteLine($"    {map.MapId}  size={map.Size} seed={map.Seed}  {map.Url}");
        }
    }

    /// <summary>Renders a generation status.</summary>
    public static void Render(Result<MapGenerationStatus> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        var status = result.Data!;
        Console.WriteLine($"  State={status.State} MapId={status.MapId} Queue={status.QueuePosition} Step={status.CurrentStep}");
    }

    /// <summary>Renders generation limits.</summary>
    public static void Render(Result<MapGenLimits> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        var limits = result.Data!;
        Console.WriteLine($"  Concurrent: {limits.Concurrent?.Current}/{limits.Concurrent?.Allowed}");
        Console.WriteLine($"  Monthly:    {limits.Monthly?.Current}/{limits.Monthly?.Allowed}");
    }

    /// <summary>Renders saved configs.</summary>
    public static void Render(Result<IReadOnlyList<MapSettings>> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        var configs = result.Data!;
        Console.WriteLine($"  {configs.Count} saved config(s):");
        foreach (var config in configs)
        {
            Console.WriteLine($"    {config.Id}  {config.Name}");
        }
    }

    /// <summary>Renders a custom-map settings tree (summary only).</summary>
    public static void Render(Result<CustomMapSettings> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        Console.WriteLine("  Custom-map settings retrieved (tree omitted for brevity).");
    }

    /// <summary>Renders an uploaded-map outcome.</summary>
    public static void Render(Result<UploadedMap> result)
    {
        if (!result.IsSuccess) { RenderFailure(result); return; }
        var map = result.Data!;
        Console.WriteLine($"  Uploaded: Id={map.Id} Name={map.DisplayName} State={map.State} Slug={map.Slug}");
    }
}
