namespace Amethyst.Protocol.Packets.Status;

public sealed class PongPacket : IOutgoingPacket
{
    public int Identifier => 1;

    int IOutgoingPacket.Length => sizeof(long);

    public required long Magic { get; init; }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteLong(Magic);
    }
}