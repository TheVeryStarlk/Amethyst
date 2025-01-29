namespace Amethyst.Protocol.Packets.Status;

public sealed class StatusResponsePacket : IOutgoingPacket
{
    public int Identifier => 0;

    int IOutgoingPacket.Length => Variable.GetByteCount(Message);

    public required string Message { get; init; }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Message);
    }
}