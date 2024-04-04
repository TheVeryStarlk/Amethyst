namespace Amethyst.Protocol.Packets.Handshaking;

internal sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0x00;

    public required int ProtocolVersion { get; init; }

    public required string Address { get; init; }

    public required ushort Port { get; init; }

    public required int NextState { get; init; }

    public static HandshakePacket Read(MemoryReader reader)
    {
        return new HandshakePacket
        {
            ProtocolVersion = reader.ReadVariableInteger(),
            Address = reader.ReadVariableString(),
            Port = reader.ReadUnsignedShort(),
            NextState = reader.ReadVariableInteger()
        };
    }
}