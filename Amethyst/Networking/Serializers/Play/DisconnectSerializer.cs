using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class DisconnectSerializer(string message) : ISerializer<DisconnectPacket, DisconnectSerializer>
{
    public int Length => Variable.GetByteCount(message);

    public static DisconnectSerializer Create(DisconnectPacket packet)
    {
        return new DisconnectSerializer(packet.Serialize());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}