namespace Amethyst.Protocol.Packets.Play;

public sealed record KeepAlivePacket(int Magic) : IIngoingPacket<KeepAlivePacket>, IOutgoingPacket
{
    public static int Identifier => 0;

    int IOutgoingPacket.Identifier => 0;

    public int Length => Variable.GetByteCount(Magic);

    static KeepAlivePacket IIngoingPacket<KeepAlivePacket>.Create(SpanReader reader)
    {
        return new KeepAlivePacket(reader.ReadVariableInteger());
    }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableInteger(Magic);
    }
}