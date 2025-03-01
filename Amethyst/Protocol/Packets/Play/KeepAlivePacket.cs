using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record KeepAlivePacket(int Magic) : IIngoingPacket<KeepAlivePacket>, IOutgoingPacket
{
    public static int Identifier => 0;

    int IOutgoingPacket.Identifier => 0;

    public int Length => Variable.GetByteCount(Magic);

    public static KeepAlivePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new KeepAlivePacket(reader.ReadVariableInteger());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableInteger(Magic);
    }
}