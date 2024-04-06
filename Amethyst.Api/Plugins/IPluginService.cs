using Amethyst.Api.Plugins.Commands;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Api.Plugins;

public interface IPluginService
{
    public IEnumerable<PluginInformation> Plugins { get; }

    public IEventService EventService { get; }

    public ICommandService CommandService { get; }
}