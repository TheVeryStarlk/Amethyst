using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class MessageSerializer(string message, byte position) : ISerializer<MessagePacket, MessageSerializer>
{
    public int Length => Variable.GetByteCount(message) + sizeof(byte);

    public static MessageSerializer Create(MessagePacket packet)
    {
        return new MessageSerializer(packet.Message.Serialize(), (byte) packet.Position);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message).WriteByte(position);
    }
}