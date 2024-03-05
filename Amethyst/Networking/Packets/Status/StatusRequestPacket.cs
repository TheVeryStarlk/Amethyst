namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusRequestPacket: IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0x00;

    public static StatusRequestPacket Read(MemoryReader reader)
    {
        return new StatusRequestPacket();
    }
}