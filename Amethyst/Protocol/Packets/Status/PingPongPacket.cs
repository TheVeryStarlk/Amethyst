using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Status;

internal sealed record PingPongPacket(long Magic) : IIngoingPacket<PingPongPacket>, IOutgoingPacket
{
    public int Length => sizeof(long);

    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => 1;

    public static PingPongPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PingPongPacket(reader.ReadLong());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(Magic);
    }
}