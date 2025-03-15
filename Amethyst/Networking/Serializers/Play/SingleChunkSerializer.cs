using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SingleChunkSerializer(SingleChunkPacket packet) : Serializer(packet)
{
    public override int Identifier => 33;

    public override int Length => sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(ushort) + Variable.GetByteCount(packet.Sections.Length) + packet.Sections.Length;

    public override void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(packet.X)
            .WriteInteger(packet.Z)
            .WriteBoolean(true)
            .WriteUnsignedShort(packet.Bitmask)
            .WriteVariableInteger(packet.Sections.Length)
            .Write(packet.Sections);
    }
}