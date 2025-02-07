namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginSuccessPacket(string Guid, string Username) : LoginSuccessPacketBase(Guid, Username), IWriteable
{
    public int Length => Variable.GetByteCount(Guid) + Variable.GetByteCount(Username);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Guid).WriteVariableString(Username);
    }
}