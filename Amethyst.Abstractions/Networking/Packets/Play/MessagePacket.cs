using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record MessagePacket(Message Message, MessagePosition Position) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(message) + sizeof(byte);

    private readonly string message = Message.Serialize();

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message).WriteByte((byte) Position);
    }
}