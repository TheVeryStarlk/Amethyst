using System.Diagnostics;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class ConfigurationPacket(string locale, byte viewDistance) : IIngoingPacket<ConfigurationPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 21;

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var configuration = dispatcher.Dispatch(client.Player, new Configuration(locale, viewDistance));

        client.Player.Locale = configuration.Locale;
        client.Player.ViewDistance = byte.Min(configuration.ViewDistance, 32);
    }
}