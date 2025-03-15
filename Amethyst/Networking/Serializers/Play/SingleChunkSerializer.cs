using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SingleChunkSerializer(int x, int z, byte[] sections, ushort bitmask) : ISerializer<SingleChunkPacket, SingleChunkSerializer>
{
    public int Length => sizeof(int)
                         + sizeof(int)
                         + sizeof(bool)
                         + sizeof(ushort)
                         + Variable.GetByteCount(sections.Length)
                         + sections.Length;

    public static SingleChunkSerializer Create(SingleChunkPacket packet)
    {
        return new SingleChunkSerializer(packet.X, packet.Z, packet.Sections, packet.Bitmask);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(x)
            .WriteInteger(z)
            .WriteBoolean(true)
            .WriteUnsignedShort(bitmask)
            .WriteVariableInteger(sections.Length)
            .Write(sections);
    }
}