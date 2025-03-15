using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class DisconnectPacket(Message message) : IOutgoingPacket
{
    public int Identifier => 64;

    public int Length => Variable.GetByteCount(message);

    private readonly string message = message.Serialize();

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}