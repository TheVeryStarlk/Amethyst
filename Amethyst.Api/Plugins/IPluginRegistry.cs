using Amethyst.Api.Commands;
using Amethyst.Api.Events;

namespace Amethyst.Api.Plugins;

public interface IPluginRegistry
{
    public void RegisterCommand(string name, Func<Command, Task> @delegate);

    public void RegisterEvent<TEventArgs>(Func<TEventArgs, Task> @delegate) where TEventArgs : AmethystEventArgsBase;
}