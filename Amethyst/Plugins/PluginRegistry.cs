using Amethyst.Api.Commands;
using Amethyst.Api.Events;
using Amethyst.Api.Plugins;
using Amethyst.Services;

namespace Amethyst.Plugins;

internal sealed class PluginRegistry(CommandService commandService, EventService eventService) : IPluginRegistry
{
    public void RegisterCommand(string name, Func<Command, Task> @delegate)
    {
        commandService.Register(name, @delegate);
    }

    public void RegisterEvent<TEventArgs>(Func<TEventArgs, Task> @delegate) where TEventArgs : AmethystEventArgsBase
    {
        eventService.Register(@delegate);
    }
}