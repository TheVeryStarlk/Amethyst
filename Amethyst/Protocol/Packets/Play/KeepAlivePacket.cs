namespace Amethyst.Protocol.Packets.Play;

internal sealed record KeepAlivePacket(int Magic) : IIngoingPacket<KeepAlivePacket>, IOutgoingPacket
{
    public static int Identifier => 0;

    int IOutgoingPacket.Identifier => 0;

    public int Length => Variable.GetByteCount(Magic);

    public static KeepAlivePacket Create(SpanReader reader)
    {
        return new KeepAlivePacket(reader.ReadVariableInteger());
    }

    public void Write(ref SpanWriter writer)
    {
        writer.WriteVariableInteger(Magic);
    }
}