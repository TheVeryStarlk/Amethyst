namespace Amethyst.Protocol.Packets.Status;

internal sealed record StatusRequestPacket : IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0;

    public static StatusRequestPacket Create(SpanReader reader)
    {
        return new StatusRequestPacket();
    }
}