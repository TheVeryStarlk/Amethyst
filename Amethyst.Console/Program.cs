using System.Text.Json;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var loaded = new AmethystServerOptions();

const string directory = "Configuration";
const string file = "configuration.json";

Directory.CreateDirectory(directory);

try
{
    var json = File.ReadAllText(Path.Combine(directory, file));
    loaded = JsonSerializer.Deserialize<AmethystServerOptions>(json)!;
}
catch
{
    File.WriteAllText(
        Path.Combine(directory, file),
        JsonSerializer.Serialize(loaded, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
}

await Host
    .CreateDefaultBuilder()
    .ConfigureLogging(configure => configure.SetMinimumLevel(LogLevel.Trace))
    .ConfigureAmethystServer((_, configuration) =>
    {
        configuration.ListeningPort = loaded.ListeningPort;
        configuration.Description = loaded.Description;
        configuration.MaximumPlayerCount = loaded.MaximumPlayerCount;
    })
    .RunConsoleAsync();