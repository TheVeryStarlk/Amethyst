using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

internal sealed class ConfigurationPacket : IIngoingPacket<ConfigurationPacket>, IDispatchable
{
    public static int Identifier => 21;

    public required string Locale { get; init; }

    public required byte ViewDistance { get; init; }

    public static ConfigurationPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new ConfigurationPacket
        {
            Locale = reader.ReadVariableString(),
            ViewDistance = reader.ReadByte()
        };
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(player, new Configuration(Locale, ViewDistance));
    }
}