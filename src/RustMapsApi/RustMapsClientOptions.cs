namespace RustMapsApi;

/// <summary>Configures a RustMaps client.</summary>
public sealed class RustMapsClientOptions
{
    /// <summary>The RustMaps API key, sent in the <c>X-API-Key</c> header.</summary>
    public string ApiKey { get; set; } = "";

    /// <summary>The API base address.</summary>
    public Uri BaseAddress { get; set; } = new("https://api.rustmaps.com");

    /// <summary>The per-request timeout.</summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}
