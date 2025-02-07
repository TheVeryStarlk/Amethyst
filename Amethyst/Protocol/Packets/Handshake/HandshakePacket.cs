namespace Amethyst.Protocol.Packets.Handshake;

internal sealed record HandshakePacket(int Version, string Address, ushort Port, int State)
    : HandshakePacketBase(Version, Address, Port, State), ICreatable<HandshakePacket>
{
    public static HandshakePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new HandshakePacket(
            reader.ReadVariableInteger(),
            reader.ReadVariableString(),
            reader.ReadUnsignedShort(),
            reader.ReadVariableInteger());
    }
}