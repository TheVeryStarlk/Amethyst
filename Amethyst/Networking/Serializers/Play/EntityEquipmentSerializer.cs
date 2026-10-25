using System.Buffers;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityEquipmentSerializer(int unique, short slot, Item item, byte[] tag) : ISerializer<EntityEquipmentPacket, EntityEquipmentSerializer>
{
    // Empty slots are only written as a type of -1.
    public int Length => Variable.GetByteCount(unique) + sizeof(short) + (item.IsEmpty ? sizeof(short) : sizeof(short) + sizeof(byte) + sizeof(short) + tag.Length);

    public static EntityEquipmentSerializer Create(EntityEquipmentPacket packet)
    {
        // Items without NBT are written as a single end tag.
        byte[] tag = [Tag.End];

        if (packet.Item.Tag is null)
        {
            return new EntityEquipmentSerializer(packet.Entity.Unique, (short) packet.Slot, packet.Item, tag);
        }

        var buffer = new ArrayBufferWriter<byte>();

        TagSerializer.Serialize(buffer, packet.Item.Tag);

        tag = [.. buffer.WrittenSpan];

        return new EntityEquipmentSerializer(packet.Entity.Unique, (short) packet.Slot, packet.Item, tag);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
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