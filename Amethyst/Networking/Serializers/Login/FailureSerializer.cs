using Amethyst.Abstractions.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class FailureSerializer(string message) : ISerializer<FailurePacket, FailureSerializer>
{
    public int Length => Variable.GetByteCount(message);

    public static FailureSerializer Create(FailurePacket packet)
    {
        return new FailureSerializer(packet.Message.Serialize());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}