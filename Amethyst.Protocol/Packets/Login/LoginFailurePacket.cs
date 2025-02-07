namespace Amethyst.Protocol.Packets.Login;

public sealed record LoginFailurePacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Reason);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Reason);
    }
}