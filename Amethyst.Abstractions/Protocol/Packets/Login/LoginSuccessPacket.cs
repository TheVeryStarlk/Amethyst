namespace Amethyst.Abstractions.Protocol.Packets.Login;

public sealed record LoginSuccessPacket(string Guid, string Username) : IOutgoingPacket
{
    public int Identifier => 2;

    int IOutgoingPacket.Length => Variable.GetByteCount(Guid) + Variable.GetByteCount(Username);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Guid);
        writer.WriteVariableString(Username);
    }
}