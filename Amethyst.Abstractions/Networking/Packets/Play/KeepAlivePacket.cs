namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class KeepAlivePacket(long magic) : IOutgoingPacket
{
    public int Length => sizeof(long);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(magic);
    }
}