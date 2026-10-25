using System.Buffers;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SetSlotSerializer(short slot, Item item, byte[] tag) : ISerializer<SetSlotPacket, SetSlotSerializer>
{
    // Empty slots are only written as a type of -1.
    public int Length => sizeof(byte) + sizeof(short) + (item.IsEmpty ? sizeof(short) : sizeof(short) + sizeof(byte) + sizeof(short) + tag.Length);

    public static SetSlotSerializer Create(SetSlotPacket packet)
    {
        // Items without NBT are written as a single end tag.
        byte[] tag = [Tag.End];

        if (packet.Item.Tag is null)
        {
            return new SetSlotSerializer(packet.Slot, packet.Item, tag);
        }
        
        var buffer = new ArrayBufferWriter<byte>();

        TagSerializer.Serialize(buffer, packet.Item.Tag);
        
        tag = [.. buffer.WrittenSpan];

        return new SetSlotSerializer(packet.Slot, packet.Item, tag);
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
            .Write(tag);
    }
}