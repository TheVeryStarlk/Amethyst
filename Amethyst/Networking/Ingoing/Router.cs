using Amethyst.Networking.Ingoing.Packets;
using Amethyst.Networking.Ingoing.Packets.Handshake;
using Amethyst.Networking.Ingoing.Packets.Login;
using Amethyst.Networking.Ingoing.Packets.Play;
using Amethyst.Networking.Ingoing.Packets.Status;
using Amethyst.Networking.Ingoing.Processors;

namespace Amethyst.Networking.Ingoing;

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
    public void Route(Client client, Packet packet)
    {
        switch (client.State, packet.Identifier)
        {
            case (State.Handshake, HandshakePacket.Identifier):
                handshake.Process(client, packet.Create<HandshakePacket>());
                break;

            case (State.Status, StatusRequestPacket.Identifier):
                status.Process(client, packet.Create<StatusRequestPacket>());
                break;

            case (State.Status, PingPacket.Identifier):
                status.Process(client, packet.Create<PingPacket>());
                break;

            case (State.Login, StartPacket.Identifier):
                login.Process(client, packet.Create<StartPacket>());
                break;

            case (State.Play, MessagePacket.Identifier):
                chat.Process(client, packet.Create<MessagePacket>());
                break;

            case (State.Play, TabRequestPacket.Identifier):
                chat.Process(client, packet.Create<TabRequestPacket>());
                break;

            case (State.Play, GroundPacket.Identifier):
                movement.Process(client, packet.Create<GroundPacket>());
                break;

            case (State.Play, PositionPacket.Identifier):
                movement.Process(client, packet.Create<PositionPacket>());
                break;

            case (State.Play, LookPacket.Identifier):
                movement.Process(client, packet.Create<LookPacket>());
                break;

            case (State.Play, PositionLookPacket.Identifier):
                movement.Process(client, packet.Create<PositionLookPacket>());
                break;

            case (State.Play, DiggingPacket.Identifier):
                block.Process(client, packet.Create<DiggingPacket>());
                break;

            case (State.Play, PlacementPacket.Identifier):
                block.Process(client, packet.Create<PlacementPacket>());
                break;

            case (State.Play, HeldItemChangePacket.Identifier):
                inventory.Process(client, packet.Create<HeldItemChangePacket>());
                break;

            case (State.Play, InventoryActionPacket.Identifier):
                inventory.Process(client, packet.Create<InventoryActionPacket>());
                break;

            case (State.Play, ConfigurationPacket.Identifier):
                player.Process(client, packet.Create<ConfigurationPacket>());
                break;

            case (State.Play, StatusPacket.Identifier):
                player.Process(client, packet.Create<StatusPacket>());
                break;

            default:
                // Unhandled play packets are ignored for now.
                if (client.State is not State.Play)
                {
                    throw new ArgumentOutOfRangeException(nameof(packet), packet.Identifier, "Unknown packet identifier.");
                }

                break;
        }
    }
}