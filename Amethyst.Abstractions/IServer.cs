using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    public IEnumerable<IWorld> Worlds { get; }

    public void Stop();
}