using System.Collections;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldManager(PlayerStore playerStore) : IWorldManager
{
    public IWorld this[string name] => worlds[name];

    public int Count => worlds.Count;

    private readonly Dictionary<string, IWorld> worlds = [];

    public void Create(string name, IGenerator generator)
    {
        var world = new World(name, generator, playerStore);
        worlds[name] = world;
    }

    public IEnumerator<IWorld> GetEnumerator()
    {
        return worlds.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}