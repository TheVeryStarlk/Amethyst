using Amethyst.Abstractions.Protocol;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Protocol.Packets.Play;

public sealed record BlockChangePacket(Position Position, Block Block) : IOutgoingPacket
{
    public int Identifier => 0x23;

    public int Length => sizeof(long) + Variable.GetByteCount(Block.Type << 4 | Block.Metadata & 15);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteLong(Position.Encode())
            .WriteVariableInteger(Block.Type << 4 | Block.Metadata & 15);
    }
}