using RustMapsApi.Samples.Shared;
using RustMapsApi.V4;

var apiKey = Environment.GetEnvironmentVariable("RUSTMAPS_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    await Console.Error.WriteLineAsync("Set the RUSTMAPS_API_KEY environment variable, then re-run.");
    return 1;
}

using var http = new HttpClient
{
    BaseAddress = new Uri("https://api.rustmaps.com")
};
http.DefaultRequestHeaders.Add("X-API-Key", apiKey);

var client = new RustMapsClient(http);
await MapsMenu.RunAsync(client);
return 0;
