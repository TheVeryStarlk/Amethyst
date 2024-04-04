using Amethyst.Api.Entities;

namespace Amethyst.Api;

public interface IServer
{
    public IEnumerable<IPlayer> Players { get; }
}