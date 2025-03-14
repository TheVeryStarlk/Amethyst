namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record KeepAlivePacket(long Magic) : IOutgoingPacket
{
    public int Length => sizeof(long);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(Magic);
    }
}