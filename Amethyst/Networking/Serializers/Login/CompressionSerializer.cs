using Amethyst.Abstractions.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class CompressionSerializer(int threshold) : ISerializer<CompressionPacket, CompressionSerializer>
{
    public int Length => Variable.GetByteCount(threshold);

    public static CompressionSerializer Create(CompressionPacket packet)
    {
        return new CompressionSerializer(packet.Threshold);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableInteger(threshold);
    }
}