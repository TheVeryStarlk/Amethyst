using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record DisconnectPacket(Message Message) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(message);

    private readonly string message = Message.Serialize();

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}