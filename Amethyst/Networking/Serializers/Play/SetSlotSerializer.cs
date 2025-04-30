using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SetSlotSerializer(short slot, Item item) : ISerializer<SetSlotPacket, SetSlotSerializer>
{
    public int Length => sizeof(byte) + sizeof(short) + sizeof(short) + sizeof(byte) + sizeof(short);

    public static SetSlotSerializer Create(SetSlotPacket packet)
    {
        return new SetSlotSerializer(packet.Slot, packet.Item);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteByte(0)
            .WriteShort(slot)
            .WriteShort(item.Type)
            .WriteByte(item.Amount)
            .WriteShort(item.Durability);
    }
}