using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions;

public static class WorldExtensions
{
    /// <summary>
    /// Sets the block and shows it to the players in the world.
    /// </summary>
    /// <param name="source">A player who already sees the block, usually the one who placed or broke it.</param>
    public static void Set(this IWorld world, Position position, Block block, IPlayer? source = null)
    {
        world[position] = block;

        var packet = new BlockPacket(position, block);

        foreach (var pair in world.Players.Where(pair => pair.Value != source))
        {
            pair.Value.Client.Write(packet);
        }
    }
}
