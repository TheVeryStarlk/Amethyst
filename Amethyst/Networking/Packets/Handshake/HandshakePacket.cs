namespace Amethyst.Networking.Packets.Handshake;

internal sealed class HandshakePacket(int version, string address, ushort port, int state) : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    public int Version => version;

    public string Address => address;

    public ushort Port => port;

    public int State => state;

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