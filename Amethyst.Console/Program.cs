using System.Net;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await Host
    .CreateDefaultBuilder()
    .ConfigureLogging(configure => configure.SetMinimumLevel(LogLevel.Trace))
    .ConfigureMinecraftServer((_, configuration) => configuration.ListeningEndPoint = new IPEndPoint(IPAddress.Any, 25565))
    .RunConsoleAsync();