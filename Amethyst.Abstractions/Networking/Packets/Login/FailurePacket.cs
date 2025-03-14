using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed class FailurePacket(Message message) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(message);

    private readonly string message = message.Serialize();

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}