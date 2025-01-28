namespace Amethyst.Protocol.Packets.Status;

public sealed class StatusRequestPacket : IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0;

    public static StatusRequestPacket Create()
    {
        return new StatusRequestPacket();
    }
}