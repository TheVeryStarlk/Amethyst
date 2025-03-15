using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class KeepAliveSerializer(KeepAlivePacket packet) : Serializer(packet)
{
    public override int Identifier => 0;

    public override int Length => sizeof(long);

    public override void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(3);
    }
}