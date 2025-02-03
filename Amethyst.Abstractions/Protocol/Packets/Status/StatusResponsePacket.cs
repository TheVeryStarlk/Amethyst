namespace Amethyst.Abstractions.Protocol.Packets.Status;

public sealed record StatusResponsePacket(string Message) : IOutgoingPacket
{
    public int Identifier => 0;

    int IOutgoingPacket.Length => Variable.GetByteCount(Message);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Message);
    }
}