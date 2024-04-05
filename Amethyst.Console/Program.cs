using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

await Host
    .CreateDefaultBuilder()
    .ConfigureLogging(configure => configure.SetMinimumLevel(LogLevel.Trace))
    .ConfigureServer((_, options) =>
    {
        options.Description = "Hello world from the console!";
    })
    .RunConsoleAsync();