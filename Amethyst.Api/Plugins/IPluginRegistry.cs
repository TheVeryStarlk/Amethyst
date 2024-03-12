using Amethyst.Api.Plugins.Events;

namespace Amethyst.Api.Plugins;

public interface IPluginRegistry
{
    public void RegisterCommand(string name, GenericDelegate<CommandContext> @delegate);

    public void RegisterEvent<T>(GenericDelegate<T> @delegate) where T : ServerEventArgsBase;
}

public delegate Task GenericDelegate<in T>(T @event);