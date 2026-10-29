using System.Diagnostics;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Play;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class PlayerProcessor(Dispatcher dispatcher) : IProcessor<ConfigurationPacket>, IProcessor<StatusPacket>
{
    public void Process(Client client, ConfigurationPacket packet)
    {
        Debug.Assert(client.Player is not null);

        var configuration = dispatcher.Dispatch(client.Player, new Configuration(packet.Locale, packet.ViewDistance));

        client.Player.Locale = configuration.Locale;
        client.Player.ViewDistance = byte.Min(configuration.ViewDistance, 32);
    }

    public void Process(Client client, StatusPacket packet)
    {
    }
}