namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginFailurePacket(string Reason) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(Reason);

    public void Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Reason);
    }
}