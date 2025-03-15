namespace Amethyst.Abstractions.Networking.Packets.Status;

// Probably should take a status class.
public sealed class StatusResponsePacket(string status) : IOutgoingPacket
{
    public int Length => Variable.GetByteCount(status);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(status);
    }
}