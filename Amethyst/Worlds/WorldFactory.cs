using System.Collections.Concurrent;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldFactory : IWorldFactory
{
    public IReadOnlyCollection<IWorld> Worlds => worlds;

    private readonly ConcurrentBag<IWorld> worlds = [];

    public IWorld Create(string name, IGenerator generator, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful)
    {
        var world = new World(name, type, dimension, difficulty, generator);
        worlds.Add(world);

        return world;
    }
}