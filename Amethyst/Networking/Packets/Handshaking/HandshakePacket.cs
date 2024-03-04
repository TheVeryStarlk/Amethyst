namespace Amethyst.Networking.Packets.Handshaking;

internal sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public int Identifier => 0x00;

    public required int ProtocolVersion { get; init; }

    public required string Address { get; init; }

    public required ushort Port { get; init; }

    public required MinecraftClientState NextState { get; init; }

    public static HandshakePacket Read(MemoryReader reader)
    {
        return new HandshakePacket
        {
            ProtocolVersion = reader.ReadVariableInteger(),
            Address = reader.ReadString(),
            Port = reader.ReadUnsignedShort(),
            NextState = (MinecraftClientState) reader.ReadVariableInteger()
        };
    }
}