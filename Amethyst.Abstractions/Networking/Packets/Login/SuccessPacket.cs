namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed class LoginSuccessPacket(string unique, string username) : IOutgoingPacket
{
    public int Identifier => 2;

    public int Length => Variable.GetByteCount(unique) + Variable.GetByteCount(username);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(unique).WriteVariableString(username);
    }
}