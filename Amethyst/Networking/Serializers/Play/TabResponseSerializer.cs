using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class TabResponseSerializer(string[] matches) : ISerializer<TabResponsePacket, TabResponseSerializer>
{
    public static TabResponseSerializer Create(TabResponsePacket packet)
    {
        return new TabResponseSerializer(packet.Matches);
    }
}