namespace Amethyst.Abstractions.Protocol.Packets.Login;

public sealed record LoginFailurePacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 0;

    int IOutgoingPacket.Length => Variable.GetByteCount(Reason);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Reason);
    }
}