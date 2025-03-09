using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IWorldStore WorldStore { get; }

    public void Stop();
}