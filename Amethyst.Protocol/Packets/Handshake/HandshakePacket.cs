namespace Amethyst.Protocol.Packets.Handshake;

public sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    static HandshakePacket IIngoingPacket<HandshakePacket>.Create(SpanReader reader)
    {
        return new HandshakePacket();
    }
}