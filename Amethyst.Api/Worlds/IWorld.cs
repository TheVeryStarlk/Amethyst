using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api.Worlds;

public interface IWorld
{
    public IServer Server { get; }

    public IEnumerable<IPlayer> Players { get; }

    public WorldType Type { get; set; }

    public Difficulty Difficulty { get; set; }

    public Dimension Dimension { get; set; }

    public Block GetBlock(Position position);

    public Task SetBlockAsync(Block block, Position position);

    public Task SpawnAsync(IEntity entity);

    public Task DestroyAsync(params IEntity[] entities);
}