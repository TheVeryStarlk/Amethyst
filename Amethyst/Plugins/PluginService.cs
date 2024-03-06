using System.Reflection;
using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugins;

internal sealed class PluginService : IAsyncDisposable
{
    public HashSet<EventWrapper> Events { get; } = [];

    private readonly HashSet<PluginBase> plugins = [];
    private readonly ILogger<PluginService> logger;
    private readonly IPluginRegistry registry;

    public PluginService(ILogger<PluginService> logger)
    {
        this.logger = logger;
        registry = new PluginRegistry(this);
    }

    public void Load()
    {
        logger.LogInformation("Loading plugins");

        var paths = Array.Empty<string>();

        try
        {
            paths = Directory.GetFiles("plugins");
        }
        catch
        {
            Directory.CreateDirectory("plugins");
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

    public async Task ExecuteEventAsync<T>(T eventArgs) where T : MinecraftEventArgsBase
    {
        foreach (var @event in Events.Where(@event => @event.EventArgs == typeof(T)))
        {
            await (Task) @event.Delegate.DynamicInvoke(eventArgs)!;
        }
    }

    public async ValueTask DisposeAsync()
    {
        var tasks = plugins.Select(plugin => plugin.DisposeAsync().AsTask());
        await Task.WhenAll(tasks);
    }
}