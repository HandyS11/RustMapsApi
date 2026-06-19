using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RustMapsApi.Samples.Shared;
using RustMapsApi.V4;

// Ensure Host.CreateApplicationBuilder resolves appsettings.json from the binary output
// directory rather than the process working directory (which is the solution root when
// running via `dotnet run`).
var hostOptions = new HostApplicationBuilderSettings
{
    ContentRootPath = AppContext.BaseDirectory
};

var builder = Host.CreateApplicationBuilder(hostOptions);

// User-secrets overrides the appsettings.json placeholder in Development.
builder.Configuration.AddUserSecrets(typeof(Program).Assembly, optional: true);

builder.Services.AddRustMapsClientV4(builder.Configuration.GetSection("RustMaps"));

try
{
    // Build triggers ValidateOnStart — throws OptionsValidationException if ApiKey is blank.
    using var host = builder.Build();

    // Resolving the client also goes through options validation.
    var client = host.Services.GetRequiredService<IRustMapsClient>();
    await MapsMenu.RunAsync(client);
    return 0;
}
catch (OptionsValidationException ex)
{
    await Console.Error.WriteLineAsync($"Configuration error: {string.Join("; ", ex.Failures)}");
    await Console.Error.WriteLineAsync(
        "Set the key with: dotnet user-secrets set \"RustMaps:ApiKey\" \"YOUR_KEY\" " +
        "--project samples/RustMapsApi.DependencyInjection.ConsoleApp");
    return 1;
}
