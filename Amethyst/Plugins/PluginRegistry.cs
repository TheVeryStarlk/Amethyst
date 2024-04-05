using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal sealed class PluginRegistry(EventService eventService) : IPluginRegistry
{
    public void RegisterEvent<TEvent>(Func<TEvent, Task> @delegate) where TEvent : MinecraftEventBase
    {
        eventService.Register(@delegate);
    }
}