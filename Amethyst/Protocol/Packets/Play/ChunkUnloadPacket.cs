using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record ChunkUnloadPacket(int X, int Z) : IOutgoingPacket
{
    public int Identifier => 33;

    public int Length => sizeof(int)
                         + sizeof(int)
                         + sizeof(bool)
                         + sizeof(ushort)
                         + Variable.GetByteCount(0);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(X)
            .WriteInteger(Z)
            .WriteBoolean(true)
            .WriteUnsignedShort(0)
            .WriteVariableInteger(0)
            .Write([]);
    }
}