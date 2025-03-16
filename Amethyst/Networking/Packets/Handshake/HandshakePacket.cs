namespace Amethyst.Networking.Packets.Handshake;

internal sealed record HandshakePacket(int Version, string Address, ushort Port, State State) : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    public static HandshakePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new HandshakePacket(
            reader.ReadVariableInteger(),
            reader.ReadVariableString(),
            reader.ReadUnsignedShort(),
            (State) reader.ReadVariableInteger());
    }
}