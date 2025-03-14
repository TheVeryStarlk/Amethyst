namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed record PongPacket(long Magic) : IOutgoingPacket
{
    public int Length => sizeof(long);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(Magic);
    }
}