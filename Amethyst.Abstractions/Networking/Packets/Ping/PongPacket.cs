namespace Amethyst.Abstractions.Networking.Packets.Ping;

public sealed class PongPacket(long magic) : IOutgoingPacket
{
    public int Length => sizeof(long);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(magic);
    }
}