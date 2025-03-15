using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class FailureSerializer(FailurePacket packet) : Serializer(packet)
{
    public override int Identifier => 0;

    public override int Length => Variable.GetByteCount(message);

    private readonly string message = packet.Message.Serialize();

    public override void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}