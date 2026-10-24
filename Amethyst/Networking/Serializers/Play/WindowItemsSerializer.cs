using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class WindowItemsSerializer(Item[] items) : ISerializer<WindowItemsPacket, WindowItemsSerializer>
{
    // Empty slots are only written as a type of -1.
    public int Length => sizeof(byte) + sizeof(short) + items.Sum(item => item.IsEmpty ? sizeof(short) : sizeof(short) + sizeof(byte) + sizeof(short) + sizeof(bool));

    public static WindowItemsSerializer Create(WindowItemsPacket packet)
    {
        return new WindowItemsSerializer(packet.Items);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteByte(0)
            .WriteShort((short) items.Length);

        foreach (var item in items)
        {
            if (item.IsEmpty)
            {
                writer.WriteShort(-1);
                continue;
            }

            writer
                .WriteShort(item.Type)
                .WriteByte(item.Amount)
                .WriteShort(item.Durability)
                .WriteBoolean(false);
        }
    }
}
