namespace Amethyst.Protocol.Packets.Status;

internal sealed record StatusResponsePacket(string Message) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Message);

    public void Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Message);
    }
}