using System.Collections.Frozen;
using Amethyst.Eventing.Player;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Play;

namespace Amethyst.Networking.Processors;

// This screams refactor me!
internal sealed class PlayProcessor : IProcessor
{
    private static readonly FrozenDictionary<int, Action<Client, Packet>> Processors = new Dictionary<int, Action<Client, Packet>>
    {
        { MessagePacket.Identifier, Message },
        { GroundPacket.Identifier, Ground },
        { PositionPacket.Identifier, Position },
        { LookPacket.Identifier, Look },
        { PositionLookPacket.Identifier, PositionLook },
        { ConfigurationPacket.Identifier, Configuration }
    }.ToFrozenDictionary();

    public static void Process(Client client, Packet packet)
    {
        if (Processors.TryGetValue(packet.Identifier, out var action))
        {
            action(client, packet);
        }
    }

    private static void Message(Client client, Packet packet)
    {
        client.EventDispatcher.Dispatch(client.Player!, new Sent(packet.Create<MessagePacket>().Message));
    }

    private static void Ground(Client client, Packet packet)
    {
        client.Player!.Ground = packet.Create<GroundPacket>().Value;
    }

    private static void Position(Client client, Packet packet)
    {
        var position = packet.Create<PositionPacket>();
        client.EventDispatcher.Dispatch(client.Player!, new Moving(position.Location, client.Player!.Yaw, client.Player.Pitch));

        client.Player!.Location = position.Location;
        client.Player.Ground = position.Ground;
    }

    private static void Look(Client client, Packet packet)
    {
        var look = packet.Create<LookPacket>();
        client.EventDispatcher.Dispatch(client.Player!, new Moving(client.Player!.Location, look.Yaw, look.Pitch));

        client.Player!.Yaw = look.Yaw;
        client.Player.Pitch = look.Pitch;
        client.Player.Ground = look.Ground;
    }

    private static void PositionLook(Client client, Packet packet)
    {
        var positionLook = packet.Create<PositionLookPacket>();
        client.EventDispatcher.Dispatch(client.Player!, new Moving(positionLook.Location, positionLook.Yaw, positionLook.Pitch));

        client.Player!.Location = positionLook.Location;
        client.Player!.Yaw = positionLook.Yaw;
        client.Player.Pitch = positionLook.Pitch;
        client.Player.Ground = positionLook.Ground;
    }

    private static void Configuration(Client client, Packet packet)
    {
        var configuration = packet.Create<ConfigurationPacket>();
        client.EventDispatcher.Dispatch(client.Player!, new Configuration(configuration.Locale, configuration.ViewDistance));

        client.Player!.Locale = configuration.Locale;
        client.Player.ViewDistance = configuration.ViewDistance;
    }
}