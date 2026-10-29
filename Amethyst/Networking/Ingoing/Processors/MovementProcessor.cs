using System.Diagnostics;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Play;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class MovementProcessor(Dispatcher dispatcher) :
    IProcessor<GroundPacket>,
    IProcessor<PositionPacket>,
    IProcessor<LookPacket>,
    IProcessor<PositionLookPacket>
{
    public void Process(Client client, GroundPacket packet)
    {
        Debug.Assert(client.Player is not null);

        client.Player.Ground = packet.Value;
    }

    public void Process(Client client, PositionPacket packet)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;

        player.Position = packet.Position;
        player.Ground = packet.Ground;

        dispatcher.Dispatch(player, new Moved(packet.Position, player.Yaw, player.Pitch));
    }

    public void Process(Client client, LookPacket packet)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;

        player.Yaw = packet.Yaw;
        player.Pitch = packet.Pitch;
        player.Ground = packet.Ground;

        dispatcher.Dispatch(player, new Moved(player.Position, packet.Yaw, packet.Pitch));
    }

    public void Process(Client client, PositionLookPacket packet)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;

        player.Position = packet.Position;
        player.Yaw = packet.Yaw;
        player.Pitch = packet.Pitch;
        player.Ground = packet.Ground;

        dispatcher.Dispatch(player, new Moved(packet.Position, packet.Yaw, packet.Pitch));
    }
}