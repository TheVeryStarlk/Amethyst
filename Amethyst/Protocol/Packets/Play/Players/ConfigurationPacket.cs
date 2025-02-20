using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play.Players;

internal sealed record ConfigurationPacket(string Locale, byte ViewDistance) : IIngoingPacket<ConfigurationPacket>, IDispatchable
{
    public static int Identifier => 21;

    public static ConfigurationPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new ConfigurationPacket(reader.ReadVariableString(), reader.ReadByte());
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        player.Locale = Locale;
        player.ViewDistance = ViewDistance;

        eventDispatcher.Dispatch(player, new Configuration(Locale, ViewDistance));
    }
}