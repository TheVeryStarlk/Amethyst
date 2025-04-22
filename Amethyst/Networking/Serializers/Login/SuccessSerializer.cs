using Amethyst.Abstractions.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class SuccessSerializer(string guid, string username) : ISerializer<SuccessPacket, SuccessSerializer>
{
    public int Length => Variable.GetByteCount(guid) + Variable.GetByteCount(username);

    public static SuccessSerializer Create(SuccessPacket packet)
    {
        return new SuccessSerializer(packet.Player.Guid.ToString(), packet.Player.Username);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(guid).WriteVariableString(username);
    }
}