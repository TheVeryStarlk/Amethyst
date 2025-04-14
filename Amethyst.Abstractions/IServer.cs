using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorldFactory WorldFactory { get; }

    public IPlayerRepository PlayerRepository { get; }
}