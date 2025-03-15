using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class DisconnectSerializer(DisconnectPacket packet) : Serializer(packet)
{
    public override int Identifier => 64;

    public override int Length => Variable.GetByteCount(message);

    private readonly string message = packet.Message.Serialize();

    public override void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}