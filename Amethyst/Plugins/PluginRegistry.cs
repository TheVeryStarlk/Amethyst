using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Commands;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal sealed class PluginRegistry(
    EventDispatcher eventDispatcher,
    CommandExecutor commandExecutor) : IPluginRegistry
{
    public void RegisterEvent<TEvent>(Func<TEvent, Task> @delegate) where TEvent : MinecraftEventBase
    {
        eventDispatcher.Register(@delegate);
    }

    public void RegisterCommand(string name, Func<CommandInformation, Task> @delegate)
    {
        commandExecutor.Register(name, @delegate);
    }
}