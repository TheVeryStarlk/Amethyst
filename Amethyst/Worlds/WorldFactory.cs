using Amethyst.Abstractions.Worlds;
using Amethyst.Entities.Players;

namespace Amethyst.Worlds;

internal sealed class WorldFactory(PlayerRepository playerRepository) : IWorldFactory
{
    public IWorld Create(string name, IGenerator generator, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful)
    {
        return new World(playerRepository, name, type, dimension, difficulty, generator);
    }
}