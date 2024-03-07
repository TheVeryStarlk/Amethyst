using Amethyst.Api.Components;

namespace Amethyst.Networking.Packets.Handshaking;

internal sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0x00;

    public required int ProtocolVersion { get; init; }

    public required string Address { get; init; }

    public required ushort Port { get; init; }

    public required MinecraftClientState NextState { get; init; }

    public static HandshakePacket Read(MemoryReader reader)
    {
        return new HandshakePacket
        {
            ProtocolVersion = reader.ReadVariableInteger(),
            Address = reader.ReadVariableString(),
            Port = reader.ReadUnsignedShort(),
            NextState = (MinecraftClientState) reader.ReadVariableInteger()
        };
    }

    public async Task HandleAsync(MinecraftClient client)
    {
        if (ProtocolVersion != MinecraftServer.ProtocolVersion
            && NextState is MinecraftClientState.Login)
        {
            await client.Transport.Output.WritePacketAsync(
                new DisconnectPacket(MinecraftClientState.Login)
                {
                    Reason = ChatMessage.Create(
                        ProtocolVersion > MinecraftServer.ProtocolVersion
                            ? "Outdated server."
                            : "Outdated client.",
                        Color.Red)
                });

            await client.StopAsync();
        }
    }
}