using System.Reflection;
using Amethyst.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class PluginService : IAsyncDisposable
{
    public CommandService CommandService { get; }

    public EventService EventService { get; }

    private readonly HashSet<PluginBase> plugins = [];
    private readonly ILogger<PluginService> logger;
    private readonly IPluginRegistry registry;

    public PluginService(ILogger<PluginService> logger, CommandService commandService, EventService eventService)
    {
        this.logger = logger;

        CommandService = commandService;
        EventService = eventService;
        registry = new PluginRegistry(this);
    }

    public void Load()
    {
        logger.LogInformation("Loading plugins");

        var paths = Array.Empty<string>();

        try
        {
            paths = Directory.GetFiles("Plugins");
        }
        catch
        {
            Directory.CreateDirectory("Plugins");
        }

        foreach (var path in paths)
        {
            var type = Assembly
                .LoadFile(Path.GetFullPath(path))
                .ExportedTypes.FirstOrDefault(type => type.BaseType == typeof(PluginBase));

            var plugin = (PluginBase) Activator.CreateInstance(type!)!;
            plugin.ConfigureRegistry(registry);
            plugins.Add(plugin);
        }

        logger.LogInformation("Finished loading plugins");
    }

    public async ValueTask DisposeAsync()
    {
        var tasks = plugins.Select(plugin => plugin.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }
}