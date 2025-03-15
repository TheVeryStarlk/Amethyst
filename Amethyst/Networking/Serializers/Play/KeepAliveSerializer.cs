using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class KeepAliveSerializer(long magic) : ISerializer<KeepAlivePacket, KeepAliveSerializer>
{
    public int Length => sizeof(long);

    public static KeepAliveSerializer Create(KeepAlivePacket packet)
    {
        return new KeepAliveSerializer(packet.Magic);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(magic);
    }
}