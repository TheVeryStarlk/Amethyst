namespace Amethyst.Protocol.Packets.Ping;

public sealed class PingPacket : IIngoingPacket<PingPacket>
{
    public static int Identifier => 1;

    public required long Magic { get; init; }

    static PingPacket IIngoingPacket<PingPacket>.Create(SpanReader reader)
    {
        return new PingPacket
        {
            Magic = reader.ReadLong()
        };
    }
}