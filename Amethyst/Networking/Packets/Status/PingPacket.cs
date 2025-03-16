namespace Amethyst.Networking.Packets.Status;

internal sealed record PingPacket(long Magic) : IIngoingPacket<PingPacket>
{
    public static int Identifier => 1;

    public static PingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PingPacket(reader.ReadLong());
    }
}