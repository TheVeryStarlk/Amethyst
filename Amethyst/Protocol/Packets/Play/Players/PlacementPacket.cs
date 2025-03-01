using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players;

internal sealed class PlacementPacket : IIngoingPacket<PlacementPacket>, IDispatchable
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        return new PlacementPacket();
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(player, new Placing());
    }
}