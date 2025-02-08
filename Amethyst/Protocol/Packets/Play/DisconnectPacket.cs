using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record DisconnectPacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 64;

    public int Length => Variable.GetByteCount(Reason);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Reason);
    }
}