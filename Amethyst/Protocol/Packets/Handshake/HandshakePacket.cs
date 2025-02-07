using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Handshake;

public sealed record HandshakePacket(int Version, string Address, ushort Port, int State) : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

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