namespace Amethyst.Protocol.Packets.Status;

public sealed record PingPongPacket(long Magic) : IIngoingPacket<PingPongPacket>, IOutgoingPacket
{
    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => Identifier;

    public int Length => sizeof(long);

    static PingPongPacket IIngoingPacket<PingPongPacket>.Create(SpanReader reader)
    {
        return new PingPongPacket(reader.ReadLong());
    }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteLong(Magic);
    }
}