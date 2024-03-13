using System.Text.Json;
using Amethyst.Api;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var configuration = new ServerConfiguration();

const string directory = "Configuration";
const string file = "configuration.json";

Directory.CreateDirectory(directory);

try
{
    var json = File.ReadAllText(Path.Combine(directory, file));
    configuration = JsonSerializer.Deserialize<ServerConfiguration>(json)!;
}
catch
{
    File.WriteAllText(
        Path.Combine(directory, file),
        JsonSerializer.Serialize(configuration, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
}

await Host
    .CreateDefaultBuilder()
    .ConfigureLogging(configure => configure.SetMinimumLevel(LogLevel.Trace))
    .ConfigureServer((_, @default) =>
    {
        @default.ListeningPort = configuration.ListeningPort;
        @default.Description = configuration.Description;
        @default.MaximumPlayerCount = configuration.MaximumPlayerCount;
    })
    .RunConsoleAsync();