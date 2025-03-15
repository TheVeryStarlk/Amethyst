using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed class PongSerializer : ISerializer<PongPacket>
{
    public int Identifier => 1;

    public int Length => sizeof(long);

    public static ISerializer Create(PongPacket packet)
    {
        return new PongSerializer();
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(4);
    }
}