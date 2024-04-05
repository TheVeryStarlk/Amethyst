using Amethyst.Api.Entities;

namespace Amethyst.Api.Worlds;

public interface IWorld
{
    public IServer Server { get; }

    public IEnumerable<IPlayer> Players { get; }

    public Task SpawnAsync(IEntity entity);

    public Task DestroyAsync(params IEntity[] entities);
}