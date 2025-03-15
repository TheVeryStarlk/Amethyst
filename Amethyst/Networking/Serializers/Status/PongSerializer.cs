using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed class PongSerializer(PongPacket packet) : Serializer(packet)
{
    public override int Identifier => 1;

    public override int Length => sizeof(long);

    public override void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(4);
    }
}