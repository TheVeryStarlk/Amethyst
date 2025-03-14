namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed record LoginSuccessPacket(string Unique, string Username) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(Unique) + Variable.GetByteCount(Username);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Unique).WriteVariableString(Username);
    }
}