using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class FailureSerializer(string message) : ISerializer<FailurePacket>
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(message);

    public static ISerializer Create(FailurePacket packet)
    {
        return new FailureSerializer(packet.Message.Serialize());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}