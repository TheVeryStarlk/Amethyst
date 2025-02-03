namespace Amethyst.Abstractions.Protocol.Packets.Handshake;

public sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0;

    public required int Version { get; init; }

    public required string Address { get; init; }

    public required ushort Port { get; init; }

    public required int State { get; init; }

    static HandshakePacket IIngoingPacket<HandshakePacket>.Create(SpanReader reader)
    {
        return new HandshakePacket
        {
            Version = reader.ReadVariableInteger(),
            Address = reader.ReadVariableString(),
            Port = reader.ReadUnsignedShort(),
            State = reader.ReadVariableInteger()
        };
    }
}