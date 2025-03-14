namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record SingleChunkPacket(int X, int Z, byte[] Sections, ushort Bitmask) : IOutgoingPacket
{
    public int Length => sizeof(int) + sizeof(int) + sizeof(bool) + sizeof(ushort) + Variable.GetByteCount(Sections.Length) + Sections.Length;

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(X)
            .WriteInteger(Z)
            .WriteBoolean(true)
            .WriteUnsignedShort(Bitmask)
            .WriteVariableInteger(Sections.Length)
            .Write(Sections);
    }
}