namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class SingleChunkPacket(int x, int z, byte[] sections, ushort bitmask) : IOutgoingPacket
{
    public int Identifier => 33;

    public int Length => sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(ushort) + Variable.GetByteCount(sections.Length) + sections.Length;

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