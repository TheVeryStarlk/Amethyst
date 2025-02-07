namespace Amethyst.Protocol.Packets.Play;

internal sealed record DisconnectPacket(string Reason) : DisconnectPacketBase(Reason), IWriteable
{
    public int Length => Variable.GetByteCount(Reason);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Reason);
    }
}