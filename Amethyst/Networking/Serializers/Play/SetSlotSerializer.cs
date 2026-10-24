using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SetSlotSerializer(short slot, Item item) : ISerializer<SetSlotPacket, SetSlotSerializer>
{
    // Empty slots are only written as a type of -1.
    public int Length => sizeof(byte) + sizeof(short) + (item.IsEmpty ? sizeof(short) : sizeof(short) + sizeof(byte) + sizeof(short) + sizeof(bool));

    public static SetSlotSerializer Create(SetSlotPacket packet)
    {
        return new SetSlotSerializer(packet.Slot, packet.Item);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteByte(0)
            .WriteShort(slot);

        if (item.IsEmpty)
        {
            writer.WriteShort(-1);
            return;
        }

        writer
            .WriteShort(item.Type)
            .WriteByte(item.Amount)
            .WriteShort(item.Durability)
            .WriteBoolean(false);
    }
}
