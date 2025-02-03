namespace Amethyst.Protocol.Packets.Status;

internal sealed record PingPongPacket(long Magic) : IIngoingPacket<PingPongPacket>, IOutgoingPacket
{
    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => Identifier;

    public int Length => sizeof(long);

    public static PingPongPacket Create(SpanReader reader)
    {
        return new PingPongPacket(reader.ReadLong());
    }

    public void Write(ref SpanWriter writer)
    {
        writer.WriteLong(Magic);
    }
}