using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorldFactory WorldFactory { get; }
}