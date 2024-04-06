using Amethyst.Api.Plugins.Commands;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Api.Plugins;

public interface IPluginService
{
    public IEnumerable<PluginInformation> Plugins { get; }

    public IEventDispatcher EventDispatcher { get; }

    public ICommandExecutor CommandExecutor { get; }
}