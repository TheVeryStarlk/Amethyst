using System.Collections;
using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal sealed class WorldStore : IWorldStore
{
    public IWorld this[string name] => worlds[name];

    public int Count => worlds.Count;

    private readonly Dictionary<string, IWorld> worlds = [];

    public void Create(string name)
    {
        var world = new World(name);
        worlds[name] = world;
    }

    public IEnumerator<IWorld> GetEnumerator()
    {
        return worlds.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return worlds.Values.GetEnumerator();
    }
}