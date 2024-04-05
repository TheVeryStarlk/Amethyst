using Microsoft.Extensions.Logging;

namespace Amethyst.Api.Plugins;

public abstract class PluginBase : IAsyncDisposable
{
    public abstract PluginInformation Information { get; }

    public required ILogger Logger { get; set; }

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}