namespace Amethyst.Protocol.Packets.Status;

public sealed record PongPacket(long Magic) : IOutgoingPacket
{
    public int Identifier => 1;

    int IOutgoingPacket.Length => sizeof(long);

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteLong(Magic);
    }
}