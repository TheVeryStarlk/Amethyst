using Amethyst.Api.Components;
using Amethyst.Extensions;
using Amethyst.Networking.Packets.Login;

namespace Amethyst.Networking.Packets.Handshaking;

internal sealed class HandshakePacket : IIngoingPacket<HandshakePacket>
{
    public static int Identifier => 0x00;

    public required int ProtocolVersion { get; init; }

    public required string Address { get; init; }

    public required ushort Port { get; init; }

    public required ClientState NextState { get; init; }

    public static HandshakePacket Read(MemoryReader reader)
    {
        return new HandshakePacket
        {
            ProtocolVersion = reader.ReadVariableInteger(),
            Address = reader.ReadVariableString(),
            Port = reader.ReadUnsignedShort(),
            NextState = (ClientState) reader.ReadVariableInteger()
        };
    }

    public async Task HandleAsync(Client client)
    {
        if (ProtocolVersion != Server.ProtocolVersion && NextState is ClientState.Login)
        {
            await client.Transport.Output.WritePacketAsync(
                new DisconnectPacket(ClientState.Login)
                {
                    Reason = ChatMessage.Create(
                        ProtocolVersion > Server.ProtocolVersion
                            ? "Outdated server."
                            : "Outdated client.",
                        Color.Red)
                });

            await client.StopAsync();
        }
    }
}