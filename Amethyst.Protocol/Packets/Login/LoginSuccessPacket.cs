namespace Amethyst.Protocol.Packets.Login;

public sealed class LoginSuccessPacket : IOutgoingPacket
{
    public int Identifier => 2;

    int IOutgoingPacket.Length => Variable.GetByteCount(Guid) + Variable.GetByteCount(Username);

    public required string Guid { get; init; }

    public required string Username { get; init; }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Guid);
        writer.WriteVariableString(Username);
    }
}