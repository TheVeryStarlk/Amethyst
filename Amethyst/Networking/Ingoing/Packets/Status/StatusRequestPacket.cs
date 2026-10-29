namespace Amethyst.Networking.Ingoing.Packets.Status;

internal sealed class StatusRequestPacket : IIngoingPacket<StatusRequestPacket>
{
    public const int Identifier = 0;

    public static StatusRequestPacket Create(ReadOnlySpan<byte> span)
    {
        return new StatusRequestPacket();
    }
}