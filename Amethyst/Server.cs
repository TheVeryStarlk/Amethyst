using Amethyst.Api;
using Amethyst.Api.Entities;

namespace Amethyst;

internal sealed class Server : IServer
{
    public IEnumerable<IPlayer> Players { get; }
}