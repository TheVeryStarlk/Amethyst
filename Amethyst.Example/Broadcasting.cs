using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets;
using Amethyst.Abstractions.Worlds;
using Amethyst.Extensions;

namespace Amethyst.Example;

internal static class Broadcasting
{
    /// <summary>
    /// Every player in every world.
    /// </summary>
    public static IEnumerable<IPlayer> Players(this IWorldFactory worldFactory)
    {
        return worldFactory.Worlds.SelectMany(world => world.Players.Values);
    }

    public static void Broadcast(this IWorldFactory worldFactory, Message message, MessagePosition position = MessagePosition.Box)
    {
        foreach (var player in worldFactory.Players())
        {
            player.Send(message, position);
        }
    }

    public static void Broadcast(this IWorldFactory worldFactory, params IOutgoingPacket[] packets)
    {
        foreach (var player in worldFactory.Players())
        {
            player.Client.Write(packets);
        }
    }
}
