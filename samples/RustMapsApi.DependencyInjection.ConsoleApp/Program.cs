using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RustMapsApi.Samples.Shared;
using RustMapsApi.V4;

// Ensure Host.CreateApplicationBuilder resolves appsettings.json (and the host environment)
// from the binary output directory rather than the process working directory (which is the
// solution root when running via `dotnet run`).
var hostOptions = new HostApplicationBuilderSettings
{
    ContentRootPath = AppContext.BaseDirectory
};

var builder = Host.CreateApplicationBuilder(hostOptions);

// User-secrets overrides the appsettings.json placeholder in Development.
builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);

// The library only rejects a blank key, so the shipped "REPLACE_ME" placeholder would
// otherwise start the app with an invalid key and fail at the first API call. Catch it here.
const string PlaceholderApiKey = "REPLACE_ME";
if (builder.Configuration["RustMaps:ApiKey"] == PlaceholderApiKey)
{
    await WriteConfigurationHelpAsync("The RustMaps API key is still the placeholder.");
    return 1;
}

builder.Services.AddRustMapsClientV4(builder.Configuration.GetSection("RustMaps"));

try
{
    // Build triggers ValidateOnStart — throws OptionsValidationException if ApiKey is blank.
    using var host = builder.Build();

    // Validation already ran during Build above; resolve the now-validated client.
    var client = host.Services.GetRequiredService<IRustMapsClient>();
    await MapsMenu.RunAsync(client);
    return 0;
}
catch (OptionsValidationException ex)
{
    await WriteConfigurationHelpAsync($"Configuration error: {string.Join("; ", ex.Failures)}");
    return 1;
}

static async Task WriteConfigurationHelpAsync(string reason)
{
    await Console.Error.WriteLineAsync(reason);
    await Console.Error.WriteLineAsync(
        "Set the key with: dotnet user-secrets set \"RustMaps:ApiKey\" \"YOUR_KEY\" " +
        "--project samples/RustMapsApi.DependencyInjection.ConsoleApp");
}
