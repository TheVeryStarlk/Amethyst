using System.Reflection;
using Amethyst.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Amethyst.Services;

internal sealed class PluginService(
    ILoggerFactory loggerFactory,
    IPluginRegistry pluginRegistry) : IAsyncDisposable
{
    private readonly ILogger<PluginService> logger = loggerFactory.CreateLogger<PluginService>();
    private readonly Dictionary<string, PluginBase> plugins = [];

    public void Initialize()
    {
        logger.LogInformation("Loading all plugins...");

        Directory.CreateDirectory("Plugins");
        var paths = Directory.GetFiles("Plugins");

        foreach (var path in paths)
        {
            var type = Assembly
                .LoadFile(Path.GetFullPath(path))
                .ExportedTypes.FirstOrDefault(type => type.BaseType == typeof(PluginBase))!;

            var plugin = (PluginBase) Activator.CreateInstance(type)!;

            if (plugins.ContainsKey(plugin.Configuration.Name))
            {
                logger.LogWarning("Found a plugin with the same name: \"{Name}\'", plugin.Configuration.Name);
                continue;
            }

            plugin.Logger = loggerFactory.CreateLogger(type);
            plugin.ConfigureRegistry(pluginRegistry);
        }
    }

    public async ValueTask DisposeAsync()
    {
        var tasks = plugins.Values.Select(plugin => plugin.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }
}