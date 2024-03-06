namespace Amethyst.Api.Plugin;

public abstract class PluginBase : IAsyncDisposable
{
    public abstract PluginConfiguration Configuration { get; }

    public abstract void ConfigureRegistry(IPluginRegistry registry);

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}