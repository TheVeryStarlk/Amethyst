namespace Amethyst.Protocol.Packets.Handshake;

public sealed record HandshakePacket(
    int Version,
    string Address,
    ushort Port,
    int State) : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    static HandshakePacket IIngoingPacket<HandshakePacket>.Create(SpanReader reader)
    {
        return new HandshakePacket(
            reader.ReadVariableInteger(),
            reader.ReadVariableString(),
            reader.ReadUnsignedShort(),
            reader.ReadVariableInteger());
    }
}