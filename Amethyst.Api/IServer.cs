using Amethyst.Api.Entities;
using Amethyst.Api.Plugins;
using Amethyst.Api.Worlds;

namespace Amethyst.Api;

public interface IServer
{
    public int ProtocolVersion { get; }

    public ServerOptions Options { get; }

    public IPluginService PluginService { get; }

    public IEnumerable<IPlayer> Players { get; }

    public IDictionary<string, IWorld> Worlds { get; }
}