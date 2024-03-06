using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal sealed class PluginRegistry(PluginService pluginService) : IPluginRegistry
{
    public void RegisterEvent<T>(GenericDelegate<T> @delegate) where T : MinecraftEventArgsBase
    {
        var wrapper = new EventWrapper(typeof(T), @delegate);
        pluginService.Events.Add(wrapper);
    }
}

internal sealed record EventWrapper(Type EventArgs, Delegate Delegate);