namespace Amethyst.Protocol.Packets.Play;

public sealed class DisconnectPacket : IOutgoingPacket
{
    public int Identifier => 64;

    int IOutgoingPacket.Length => Variable.GetByteCount(Reason);

    public required string Reason { get; init; }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Reason);
    }
}