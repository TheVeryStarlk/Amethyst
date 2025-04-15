using Amethyst.Eventing;
using Amethyst.Networking.Packets.Handshake;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Play;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking.Packets;

internal interface IProcessor
{
    public void Process(Client client, EventDispatcher eventDispatcher);
}

internal sealed class EmptyProcessor : IProcessor
{
    public static EmptyProcessor Instance { get; } = new();

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        // Will remove this later.
    }
}

internal static class PacketExtensions
{
    public static IProcessor Create(this Packet packet, State state)
    {
        // Rewrite this when switch expressions get better.
        return state switch
        {
            // Handshake.
            State.Handshake => packet switch
            {
                _ when HandshakePacket.Identifier == packet.Identifier => packet.Create<HandshakePacket>(),
                _ => throw new ArgumentOutOfRangeException(nameof(packet.Identifier))
            },

            // Status.
            State.Status => packet switch
            {
                _ when StatusRequestPacket.Identifier == packet.Identifier => packet.Create<StatusRequestPacket>(),
                _ when PingPacket.Identifier == packet.Identifier => packet.Create<PingPacket>(),
                _ => throw new ArgumentOutOfRangeException(nameof(packet.Identifier))
            },

            // Login.
            State.Login => packet switch
            {
                _ when StartPacket.Identifier == packet.Identifier => packet.Create<StartPacket>(),
                _ => throw new ArgumentOutOfRangeException(nameof(packet.Identifier))
            },

            // Play.
            State.Play => packet switch
            {
                _ when MessagePacket.Identifier == packet.Identifier => packet.Create<MessagePacket>(),
                _ when GroundPacket.Identifier == packet.Identifier => packet.Create<GroundPacket>(),
                _ when PositionPacket.Identifier == packet.Identifier => packet.Create<PositionPacket>(),
                _ when LookPacket.Identifier == packet.Identifier => packet.Create<LookPacket>(),
                _ when PositionLookPacket.Identifier == packet.Identifier => packet.Create<PositionLookPacket>(),
                _ when DiggingPacket.Identifier == packet.Identifier => packet.Create<DiggingPacket>(),
                _ when PlacementPacket.Identifier == packet.Identifier => packet.Create<PlacementPacket>(),
                _ when TabRequestPacket.Identifier == packet.Identifier => packet.Create<TabRequestPacket>(),
                _ when ConfigurationPacket.Identifier == packet.Identifier => packet.Create<ConfigurationPacket>(),
                _ when StatusPacket.Identifier == packet.Identifier => packet.Create<StatusPacket>(),
                _ => EmptyProcessor.Instance
            },
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };
    }
}