using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Commands;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal sealed class PluginRegistry(
    EventService eventService,
    CommandService commandService) : IPluginRegistry
{
    public void RegisterEvent<TEvent>(Func<TEvent, Task> @delegate) where TEvent : MinecraftEventBase
    {
        eventService.Register(@delegate);
    }

    public void RegisterCommand(string name, Func<CommandInformation, Task> @delegate)
    {
        commandService.Register(name, @delegate);
    }
}