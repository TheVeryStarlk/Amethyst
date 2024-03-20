using System.Reflection;
using Amethyst.Api.Plugins;
using Amethyst.Plugins;
using Microsoft.Extensions.Logging;

namespace Amethyst.Services;

// Maybe move this to the plugin project?
internal sealed class PluginService(
    ILoggerFactory loggerFactory,
    IPluginRegistry pluginRegistry) : IAsyncDisposable
{
    public Dictionary<string, PluginBase> Plugins { get; } = [];

    private readonly ILogger<PluginService> logger = loggerFactory.CreateLogger<PluginService>();

    public void Initialize()
    {
        logger.LogInformation("Loading all plugins...");

        var @default = new DefaultPlugin
        {
            Logger = loggerFactory.CreateLogger<DefaultPlugin>()
        };

        @default.ConfigureRegistry(pluginRegistry);
        Plugins[@default.Configuration.Name] = @default;

        Directory.CreateDirectory("Plugins");
        var paths = Directory.GetFiles("Plugins");

        foreach (var path in paths)
        {
            var type = Assembly
                .LoadFile(Path.GetFullPath(path))
                .ExportedTypes.FirstOrDefault(type => type.BaseType == typeof(PluginBase))!;

            var plugin = (PluginBase) Activator.CreateInstance(type)!;

            if (!Plugins.TryAdd(plugin.Configuration.Name, plugin))
            {
                logger.LogWarning("Found a plugin with the same name: \"{Name}\"", plugin.Configuration.Name);
                continue;
            }

            plugin.Logger = loggerFactory.CreateLogger(type);
            plugin.ConfigureRegistry(pluginRegistry);
        }
    }

    public async ValueTask DisposeAsync()
    {
        var tasks = Plugins.Values.Select(plugin => plugin.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }
}