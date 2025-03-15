using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class SuccessSerializer(string unique, string username) : ISerializer<SuccessPacket>
{
    public int Length => Variable.GetByteCount(unique) + Variable.GetByteCount(username);

    public static ISerializer Create(SuccessPacket packet)
    {
        return new SuccessSerializer(packet.Unique, packet.Username);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(unique).WriteVariableString(username);
    }
}