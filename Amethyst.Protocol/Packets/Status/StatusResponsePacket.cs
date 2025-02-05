namespace Amethyst.Protocol.Packets.Status;

public sealed record StatusResponsePacket(string Message) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Message);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Message);
    }
}