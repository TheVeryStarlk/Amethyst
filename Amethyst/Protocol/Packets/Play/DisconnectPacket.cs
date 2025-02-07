using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play;

internal sealed record DisconnectPacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 64;

    public int Length => Variable.GetByteCount(Reason);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Reason);
    }
}