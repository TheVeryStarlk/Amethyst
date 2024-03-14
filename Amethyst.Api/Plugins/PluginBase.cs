using Microsoft.Extensions.Logging;

namespace Amethyst.Api.Plugins;

public abstract class PluginBase : IAsyncDisposable
{
    public abstract PluginConfiguration Configuration { get; }

    public ILogger? Logger { get; set; }

    public abstract void ConfigureRegistry(IPluginRegistry pluginRegistry);

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}

public sealed class PluginConfiguration
{
    public required string Name { get; init; }

    public required string Description { get; init; }
}