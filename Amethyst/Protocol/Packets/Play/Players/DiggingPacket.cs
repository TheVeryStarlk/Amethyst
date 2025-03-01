using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Abstractions.Protocol;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players;

internal sealed record DiggingPacket(DiggingStatus Status, Position Position, BlockFace Face) : IIngoingPacket<DiggingPacket>, IDispatchable
{
    public static int Identifier => 7;

    public static DiggingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new DiggingPacket((DiggingStatus) reader.ReadByte(), Position.From(reader.ReadLong()), (BlockFace) reader.ReadByte());
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(player, new Digging(Status, Position, Face));
    }
}