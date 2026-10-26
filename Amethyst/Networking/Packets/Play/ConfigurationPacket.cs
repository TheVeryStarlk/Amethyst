using System.Diagnostics;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class ConfigurationPacket(string locale, byte viewDistance) : IIngoingPacket<ConfigurationPacket>, IProcessor
{
    public static int Identifier => 21;

    public static ConfigurationPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new ConfigurationPacket(reader.ReadVariableString(), reader.ReadByte());
    }

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var configuration = dispatcher.Dispatch(client.Player, new Configuration(locale, viewDistance));

        client.Player.Locale = configuration.Locale;
        client.Player.ViewDistance = byte.Min(configuration.ViewDistance, 32);
    }
}