using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;
using Amethyst.Extensions;

namespace Amethyst.Example.Generation;

/// <summary>
/// Draws straight into the world, showing every block to the players in it.
/// </summary>
internal sealed class WorldCanvas(IWorld world) : ICanvas
{
    public Block this[int x, int y, int z]
    {
        get => world[x, y, z];
        set => world.Set(new Position(x, y, z), value);
    }
}
