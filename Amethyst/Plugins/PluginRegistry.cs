using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal sealed class PluginRegistry(PluginService pluginService) : IPluginRegistry
{
    public void RegisterCommand(string name, GenericDelegate<CommandContext> @delegate)
    {
        var wrapper = new CommandWrapper(name, @delegate);
        pluginService.CommandService.Commands.Add(wrapper);
    }

    public void RegisterEvent<T>(GenericDelegate<T> @delegate) where T : ServerEventArgsBase
    {
        var wrapper = new EventWrapper(typeof(T), @delegate);
        pluginService.EventService.Events.Add(wrapper);
    }
}