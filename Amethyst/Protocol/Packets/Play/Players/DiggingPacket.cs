using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players;

internal sealed class DiggingPacket : IIngoingPacket<DiggingPacket>, IDispatchable
{
    public static int Identifier => 7;

    public static DiggingPacket Create(ReadOnlySpan<byte> span)
    {
        return new DiggingPacket();
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(player, new Digging());
    }
}