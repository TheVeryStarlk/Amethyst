namespace Amethyst.Protocol.Packets.Play;

internal sealed record KeepAlivePacket(int Magic) : KeepAlivePacketBase(Magic), ICreatable<KeepAlivePacketBase>, IWriteable
{
    public int Length => Variable.GetByteCount(Magic);

    public static KeepAlivePacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new KeepAlivePacket(reader.ReadVariableInteger());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableInteger(Magic);
    }
}