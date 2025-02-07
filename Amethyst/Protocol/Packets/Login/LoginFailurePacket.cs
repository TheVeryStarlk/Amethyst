namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginFailurePacket(string Reason) : LoginFailurePacketBase(Reason), IWriteable
{
    public int Length => Variable.GetByteCount(Reason);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Reason);
    }
}