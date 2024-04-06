using System.Reflection;
using Amethyst.Api.Plugins;
using Amethyst.Extensions;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class PluginService(
    ILoggerFactory loggerFactory,
    IPluginRegistry pluginRegistry) : IPluginService, IAsyncDisposable
{
    public IEnumerable<PluginInformation> Plugins => plugins.Values.Select(plugin => plugin.Information);

    private readonly Dictionary<string, PluginBase> plugins = [];
    private readonly ILogger<PluginService> logger = loggerFactory.CreateLogger<PluginService>();

    private const string Folder = "Plugins";

    public void Register()
    {
        Directory.CreateDirectory(Folder);

        foreach (var path in Directory.GetFiles(Folder))
        {
            try
            {
                var type = Assembly
                    .LoadFile(Path.GetFullPath(path))
                    .ExportedTypes.FirstOrDefault(type => type.BaseType == typeof(PluginBase))!;

                var plugin = (PluginBase) Activator.CreateInstance(type)!;

                if (!plugins.TryAdd(plugin.Information.Name, plugin))
                {
                    continue;
                }

                plugin.Logger = loggerFactory.CreateLogger(type);
                plugin.ConfigureRegistry(pluginRegistry);
            }
            catch
            {
                logger.LogWarning("Could not load plugin: \"{Path}\"", path);
            }
        }

        if (plugins.Count != 0)
        {
            logger.LogInformation("Finished registering all plugins");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await plugins.Values
            .Select(plugin => plugin.DisposeAsync().AsTask())
            .WhenEach();
    }
}