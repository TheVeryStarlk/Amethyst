using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class SuccessSerializer(SuccessPacket packet) : Serializer(packet)
{
    public override int Identifier => 2;

    public override int Length => Variable.GetByteCount(packet.Unique) + Variable.GetByteCount(packet.Username);

    public override void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(packet.Unique).WriteVariableString(packet.Username);
    }
}