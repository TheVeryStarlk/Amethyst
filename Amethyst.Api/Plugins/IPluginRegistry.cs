using Amethyst.Api.Plugins.Events;

namespace Amethyst.Api.Plugins;

public interface IPluginRegistry
{
    public void RegisterEvent<TEvent>(Func<TEvent, Task> @delegate) where TEvent : MinecraftEventBase;
}