using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Status;

internal sealed record StatusRequestPacket : IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0;

    public static StatusRequestPacket Create(ReadOnlySpan<byte> span)
    {
        return new StatusRequestPacket();
    }
}