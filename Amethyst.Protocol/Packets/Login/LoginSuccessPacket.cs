namespace Amethyst.Protocol.Packets.Login;

public sealed record LoginSuccessPacket(string Guid, string Username) : IOutgoingPacket
{
    public int Identifier => 2;

    public int Length => Variable.GetByteCount(Guid) + Variable.GetByteCount(Username);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Guid);
        writer.WriteVariableString(Username);
    }
}