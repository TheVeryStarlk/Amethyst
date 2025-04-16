using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class ListHeaderFooterSerializer(string header, string footer) : ISerializer<ListHeaderFooterPacket, ListHeaderFooterSerializer>
{
    public int Length => Variable.GetByteCount(header) + Variable.GetByteCount(footer);

    public static ListHeaderFooterSerializer Create(ListHeaderFooterPacket packet)
    {
        return new ListHeaderFooterSerializer(packet.Header.Serialize(), packet.Footer.Serialize());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(header).WriteVariableString(footer);
    }
}