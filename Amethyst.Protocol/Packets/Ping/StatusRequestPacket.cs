namespace Amethyst.Protocol.Packets.Ping;

public sealed class StatusRequestPacket : IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0;

    static StatusRequestPacket IIngoingPacket<StatusRequestPacket>.Create(SpanReader reader)
    {
        return new StatusRequestPacket();
    }
}