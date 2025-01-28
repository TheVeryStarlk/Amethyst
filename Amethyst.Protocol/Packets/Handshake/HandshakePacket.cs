namespace Amethyst.Protocol.Packets.Handshake;

public sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    public static HandshakePacket Create()
    {
        return new HandshakePacket();
    }
}