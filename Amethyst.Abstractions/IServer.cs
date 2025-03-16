using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions;

public interface IServer
{
    // Probably should just be an injectable class?
    public IWorldCollection Worlds { get; }

    public void Stop();
}