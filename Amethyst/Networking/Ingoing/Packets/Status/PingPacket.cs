namespace Amethyst.Networking.Ingoing.Packets.Status;

internal sealed class PingPacket(long magic) : IIngoingPacket<PingPacket>
{
    public const int Identifier = 1;

    public long Magic => magic;

    public static PingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PingPacket(reader.ReadLong());
    }
}