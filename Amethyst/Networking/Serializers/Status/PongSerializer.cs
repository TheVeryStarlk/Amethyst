using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed class PongSerializer(long magic) : ISerializer<PongPacket, PongSerializer>
{
    public int Length => sizeof(long);

    public static PongSerializer Create(PongPacket packet)
    {
        return new PongSerializer(packet.Magic);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(magic);
    }
}