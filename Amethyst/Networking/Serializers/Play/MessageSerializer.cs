using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class MessageSerializer(string message, byte position) : ISerializer<MessagePacket, MessageSerializer>
{
    public static MessageSerializer Create(MessagePacket packet)
    {
        return new MessageSerializer(packet.Message.Serialize(), (byte) packet.Position);
    }
}