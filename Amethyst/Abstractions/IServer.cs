using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IEnumerable<IPlayer> Players { get; }

    public void Stop();
}