namespace Amethyst.Protocol.Packets.Status;

internal sealed record PingPongPacket(long Magic) : PingPongPacketBase(Magic), ICreatable<PingPongPacketBase>, IWriteable
{
    public int Length => sizeof(long);

    public static PingPongPacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PingPongPacket(reader.ReadLong());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(Magic);
    }
}