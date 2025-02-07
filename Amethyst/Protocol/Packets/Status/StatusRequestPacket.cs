namespace Amethyst.Protocol.Packets.Status;

internal sealed record StatusRequestPacket : StatusRequestPacketBase, ICreatable<StatusRequestPacketBase>
{
    public static StatusRequestPacketBase Create(ReadOnlySpan<byte> span)
    {
        return new StatusRequestPacket();
    }
}