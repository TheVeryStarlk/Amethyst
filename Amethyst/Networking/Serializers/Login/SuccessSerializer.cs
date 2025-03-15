using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class SuccessSerializer(SuccessPacket packet) : ISerializer<SuccessPacket>
{
    public int Identifier => 2;

    public int Length => Variable.GetByteCount(packet.Unique) + Variable.GetByteCount(packet.Username);

    public static ISerializer Create(SuccessPacket packet)
    {
        return new SuccessSerializer(packet);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(packet.Unique).WriteVariableString(packet.Username);
    }
}