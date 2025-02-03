namespace Amethyst.Abstractions.Protocol.Packets.Play;

public sealed record DisconnectPacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 64;

    int IOutgoingPacket.Length => Variable.GetByteCount(Reason);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Reason);
    }
}