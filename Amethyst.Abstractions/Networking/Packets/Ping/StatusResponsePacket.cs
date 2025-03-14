using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Ping;

public sealed class StatusResponsePacket(Status status) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(serialized);

    private readonly string serialized = status.Serialize();

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(serialized);
    }
}