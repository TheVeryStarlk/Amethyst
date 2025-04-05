using System.Collections.Frozen;
using Amethyst.Eventing.Player;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Play;

namespace Amethyst.Networking.Processors;

internal sealed class PlayProcessor : IProcessor
{
    private static readonly FrozenDictionary<int, Action<Client, Packet>> Processors = new Dictionary<int, Action<Client, Packet>>
    {
        { MessagePacket.Identifier, Message },
        { GroundPacket.Identifier, Ground },
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

    private static void Configuration(Client client, Packet packet)
    {
        var configuration = packet.Create<ConfigurationPacket>();
        client.EventDispatcher.Dispatch(client.Player!, new Configuration(configuration.Locale, configuration.ViewDistance));

        client.Player!.Locale = configuration.Locale;
        client.Player.ViewDistance = configuration.ViewDistance;
    }
}