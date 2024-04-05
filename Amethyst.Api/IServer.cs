using Amethyst.Api.Entities;
using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events;
using Amethyst.Api.Worlds;

namespace Amethyst.Api;

public interface IServer
{
    public int ProtocolVersion { get; }

    public ServerOptions Options { get; }

    public IEnumerable<IPlayer> Players { get; }

    public IDictionary<string, IWorld> Worlds { get; }

    public IPluginService PluginService { get; }

    public IEventService EventService { get; }
}