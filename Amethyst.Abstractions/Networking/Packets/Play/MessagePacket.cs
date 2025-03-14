using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class MessagePacket(Message message, MessagePosition position) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(message) + sizeof(byte);

    private readonly string message = message.Serialize();

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message).WriteByte((byte) position);
    }
}