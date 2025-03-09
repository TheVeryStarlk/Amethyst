using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldStore(Func<string, IGenerator, IWorld> worldFactory) : IWorldStore
{
    public IReadOnlyDictionary<string, IWorld> Worlds => worlds;

    private readonly Dictionary<string, IWorld> worlds = [];

    public void Create(string name, IGenerator generator)
    {
        var world = worldFactory(name, generator);
        worlds[name] = world;
    }
}