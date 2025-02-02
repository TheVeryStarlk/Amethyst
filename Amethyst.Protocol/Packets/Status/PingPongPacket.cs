namespace Amethyst.Protocol.Packets.Status;

public sealed class PingPongPacket : IIngoingPacket<PingPongPacket>, IOutgoingPacket
{
    public static int Identifier => 1;

    public required long Magic { get; init; }

    int IOutgoingPacket.Identifier => Identifier;

    public int Length => sizeof(long);

    static PingPongPacket IIngoingPacket<PingPongPacket>.Create(SpanReader reader)
    {
        return new PingPongPacket
        {
            Magic = reader.ReadLong()
        };
    }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteLong(Magic);
    }
}