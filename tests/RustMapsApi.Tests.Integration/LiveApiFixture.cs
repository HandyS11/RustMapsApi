using System.Net.Http;
using RustMapsApi.V4;

namespace RustMapsApi.Tests.Integration;

/// <summary>
/// Shared state for live API tests: a single throttled client built from environment
/// configuration. When no API key is present the fixture still constructs, but
/// <see cref="KeyAvailable"/> is false so every test self-skips.
/// </summary>
public sealed class LiveApiFixture : IAsyncLifetime, IDisposable
{
    private readonly int _delayMs;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly HttpClient? _http;

    public LiveApiFixture()
    {
        var apiKey = Environment.GetEnvironmentVariable("RUSTMAPS_API_KEY");
        KeyAvailable = !string.IsNullOrWhiteSpace(apiKey);
        FilterId = Environment.GetEnvironmentVariable("RUSTMAPS_TEST_FILTER_ID");
        AnchorSize = ReadInt("RUSTMAPS_TEST_SIZE", 4500);
        AnchorSeed = ReadInt("RUSTMAPS_TEST_SEED", 1803363826);
        _delayMs = ReadInt("RUSTMAPS_TEST_DELAY_MS", 1000);

        if (!KeyAvailable)
        {
            return;
        }

        _http = new HttpClient
        {
            BaseAddress = new Uri("https://api.rustmaps.com")
        };
        _http.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        Client = new RustMapsClient(_http);
    }

    public bool KeyAvailable { get; }

    public RustMapsClient Client { get; } = null!;

    public string? FilterId { get; }

    public int AnchorSize { get; }

    public int AnchorSeed { get; }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        Dispose();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _http?.Dispose();
        _gate.Dispose();
    }

    /// <summary>
    /// Serializes live calls and spaces them apart. Combined with the non-parallel
    /// collection this guarantees at most one in-flight request, spaced by the delay.
    /// </summary>
    public async Task ThrottleAsync()
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            await Task.Delay(_delayMs).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static int ReadInt(string name, int fallback) =>
        int.TryParse(Environment.GetEnvironmentVariable(name), out var value) ? value : fallback;
}
