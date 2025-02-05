namespace Amethyst.Protocol.Packets.Login;

public sealed record LoginFailurePacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Reason);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Reason);
    }
}