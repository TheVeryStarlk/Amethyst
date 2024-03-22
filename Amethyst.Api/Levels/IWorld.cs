using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Levels.Blocks;
using Amethyst.Api.Levels.Generators;

namespace Amethyst.Api.Levels;

public interface IWorld
{
    public IWorldGenerator Generator { get; set; }

    public WorldType Type { get; set; }

    public Difficulty Difficulty { get; set; }

    public Dimension Dimension { get; set; }

    public long Age { get; }

    public long Time { get; set; }

    public IEnumerable<IRegion> Regions { get; }

    public Task SpawnPlayerAsync(IPlayer player);

    public Task DestroyEntitiesAsync(params IEntity[] entities);

    public Block GetBlock(Position position);

    public void SetBlock(Block block, Position position);

    public IChunk GetChunk(Position position);
}