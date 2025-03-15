using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed class PongSerializer(PongPacket packet) : ISerializer<PongPacket>
{
    public int Length => sizeof(long);

    public static ISerializer Create(PongPacket packet)
    {
        return new PongSerializer(packet);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(packet.Magic);
    }
}