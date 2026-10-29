using System.Collections.Frozen;
using Amethyst.Networking.Ingoing.Packets;
using Amethyst.Networking.Ingoing.Packets.Handshake;
using Amethyst.Networking.Ingoing.Packets.Login;
using Amethyst.Networking.Ingoing.Packets.Play;
using Amethyst.Networking.Ingoing.Packets.Status;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class Router(
    HandshakeProcessor handshake,
    StatusProcessor status,
    LoginProcessor login,
    MovementProcessor movement,
    ChatProcessor chat,
    BlockProcessor block,
    InventoryProcessor inventory,
    PlayerProcessor player)
{
    private readonly FrozenDictionary<(State State, int Identifier), Action<Client, Packet>> routes = new Dictionary<(State State, int Identifier), Action<Client, Packet>>
    {
        // Handshake.
        { (State.Handshake, HandshakePacket.Identifier), (client, packet) => handshake.Process(client, packet.Create<HandshakePacket>()) },

        // Status.
        { (State.Status, StatusRequestPacket.Identifier), (client, packet) => status.Process(client, packet.Create<StatusRequestPacket>()) },
        { (State.Status, PingPacket.Identifier), (client, packet) => status.Process(client, packet.Create<PingPacket>()) },

        // Login.
        { (State.Login, StartPacket.Identifier), (client, packet) => login.Process(client, packet.Create<StartPacket>()) },

        // Play.
        { (State.Play, MessagePacket.Identifier), (client, packet) => chat.Process(client, packet.Create<MessagePacket>()) },
        { (State.Play, TabRequestPacket.Identifier), (client, packet) => chat.Process(client, packet.Create<TabRequestPacket>()) },
        { (State.Play, GroundPacket.Identifier), (client, packet) => movement.Process(client, packet.Create<GroundPacket>()) },
        { (State.Play, PositionPacket.Identifier), (client, packet) => movement.Process(client, packet.Create<PositionPacket>()) },
        { (State.Play, LookPacket.Identifier), (client, packet) => movement.Process(client, packet.Create<LookPacket>()) },
        { (State.Play, PositionLookPacket.Identifier), (client, packet) => movement.Process(client, packet.Create<PositionLookPacket>()) },
        { (State.Play, DiggingPacket.Identifier), (client, packet) => block.Process(client, packet.Create<DiggingPacket>()) },
        { (State.Play, PlacementPacket.Identifier), (client, packet) => block.Process(client, packet.Create<PlacementPacket>()) },
        { (State.Play, HeldItemChangePacket.Identifier), (client, packet) => inventory.Process(client, packet.Create<HeldItemChangePacket>()) },
        { (State.Play, InventoryActionPacket.Identifier), (client, packet) => inventory.Process(client, packet.Create<InventoryActionPacket>()) },
        { (State.Play, ConfigurationPacket.Identifier), (client, packet) => player.Process(client, packet.Create<ConfigurationPacket>()) },
        { (State.Play, StatusPacket.Identifier), (client, packet) => player.Process(client, packet.Create<StatusPacket>()) }
    }.ToFrozenDictionary();

    public void Route(Client client, Packet packet)
    {
        if (routes.TryGetValue((client.State, packet.Identifier), out var route))
        {
            route(client, packet);
            return;
        }

        // Unhandled play packets are ignored for now.
        if (client.State is not State.Play)
        {
            throw new ArgumentOutOfRangeException(nameof(packet), packet.Identifier, "Unknown packet identifier.");
        }
    }
}