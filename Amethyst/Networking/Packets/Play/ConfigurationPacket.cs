using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class ConfigurationPacket(string locale, byte viewDistance) : IIngoingPacket<ConfigurationPacket>, IProcessor
{
    public static int Identifier => 21;

    public static ConfigurationPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new ConfigurationPacket(reader.ReadVariableString(), reader.ReadByte());
    }

    public void Process(Player player, EventDispatcher eventDispatcher)
    {
        player.Locale = locale;
        player.ViewDistance = viewDistance;
    }
}