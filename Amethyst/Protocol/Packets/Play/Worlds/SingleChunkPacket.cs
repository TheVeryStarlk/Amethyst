using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play.Worlds;

public sealed record SingleChunkPacket(int X, int Z, byte[] Chunk, ushort Bitmask) : IOutgoingPacket
{
    public int Identifier => 33;

    public int Length => sizeof(int)
                         + sizeof(int)
                         + sizeof(bool)
                         + sizeof(ushort)
                         + Variable.GetByteCount(Chunk.Length)
                         + Chunk.Length;

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(X)
            .WriteInteger(Z)
            .WriteBoolean(true)
            .WriteUnsignedShort(Bitmask)
            .WriteVariableInteger(Chunk.Length)
            .Write(Chunk);
    }
}