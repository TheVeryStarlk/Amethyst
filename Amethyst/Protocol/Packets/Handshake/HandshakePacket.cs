namespace Amethyst.Protocol.Packets.Handshake;

internal sealed record HandshakePacket(
    int Version,
    string Address,
    ushort Port,
    int State) : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    public static HandshakePacket Create(SpanReader reader)
    {
        return new HandshakePacket(
            reader.ReadVariableInteger(),
            reader.ReadVariableString(),
            reader.ReadUnsignedShort(),
            reader.ReadVariableInteger());
    }
}