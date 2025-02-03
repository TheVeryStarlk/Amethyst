namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginSuccessPacket(string Guid, string Username) : IOutgoingPacket
{
    public int Identifier => 2;

    public int Length => Variable.GetByteCount(Guid) + Variable.GetByteCount(Username);

    public void Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Guid);
        writer.WriteVariableString(Username);
    }
}