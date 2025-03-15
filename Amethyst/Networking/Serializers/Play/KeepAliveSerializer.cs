using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class KeepAliveSerializer(long magic) : ISerializer<KeepAlivePacket>
{
    public int Length => sizeof(long);

    public static ISerializer Create(KeepAlivePacket packet)
    {
        return new KeepAliveSerializer(packet.Magic);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(magic);
    }
}