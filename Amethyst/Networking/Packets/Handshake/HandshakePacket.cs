namespace Amethyst.Networking.Packets.Handshake;

internal sealed class HandshakePacket(int version, string address, ushort port, State state) : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    public int Version => version;

    public string Address => address;

    public ushort Port => port;

    public State State => state;

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

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}