using System.Buffers;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class WindowItemsSerializer(Item[] items, byte[][] tags) : ISerializer<WindowItemsPacket, WindowItemsSerializer>
{
    // Empty slots are only written as a type of -1.
    public int Length => sizeof(byte) + sizeof(short) + items.Select((item, index) => item.IsEmpty ? sizeof(short) : sizeof(short) + sizeof(byte) + sizeof(short) + tags[index].Length).Sum();

    public static WindowItemsSerializer Create(WindowItemsPacket packet)
    {
        var tags = new byte[packet.Items.Length][];

        for (var index = 0; index < tags.Length; index++)
        {
            var tag = packet.Items[index].Tag;

            // Items without NBT are written as a single end tag.
            if (tag is null)
            {
                tags[index] = [Tag.End];
                continue;
            }

            var buffer = new ArrayBufferWriter<byte>();

            TagSerializer.Serialize(buffer, tag);
            
            tags[index] = [.. buffer.WrittenSpan];
        }

        return new WindowItemsSerializer(packet.Items, tags);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteByte(0)
            .WriteShort((short) items.Length);

        for (var index = 0; index < items.Length; index++)
        {
            var item = items[index];

            if (item.IsEmpty)
            {
                writer.WriteShort(-1);
                continue;
            }

            writer
                .WriteShort(item.Type)
                .WriteByte(item.Amount)
                .WriteShort(item.Durability)
                .Write(tags[index]);
        }
    }
}