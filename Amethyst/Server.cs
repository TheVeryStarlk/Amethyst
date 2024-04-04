using Amethyst.Api;
using Amethyst.Api.Entities;

namespace Amethyst;

internal sealed class Server : IServer
{
    public int ProtocolVersion => 47;

    public ServerOptions Options { get; }

    public IEnumerable<IPlayer> Players { get; }
}