using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class ListHeaderFooterSerializer(string header, string footer) : ISerializer<ListHeaderFooterPacket, ListHeaderFooterSerializer>
{
    public static ListHeaderFooterSerializer Create(ListHeaderFooterPacket packet)
    {
        return new ListHeaderFooterSerializer(packet.Header.Serialize(), packet.Footer.Serialize());
    }
}