using Amethyst.Api.Entities;

namespace Amethyst.Api;

public interface IServer
{
    public int ProtocolVersion { get; }

    public ServerOptions Options { get; }

    public IEnumerable<IPlayer> Players { get; }
}