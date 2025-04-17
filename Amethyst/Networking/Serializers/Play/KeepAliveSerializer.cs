using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class KeepAliveSerializer(int magic) : ISerializer<KeepAlivePacket, KeepAliveSerializer>
{
    public int Length => Variable.GetByteCount(magic);

    public static KeepAliveSerializer Create(KeepAlivePacket packet)
    {
        return new KeepAliveSerializer(packet.Magic);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableInteger(magic);
    }
}