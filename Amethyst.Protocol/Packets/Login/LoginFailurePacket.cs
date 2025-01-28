namespace Amethyst.Protocol.Packets.Login;

public sealed class LoginFailurePacket : IOutgoingPacket
{
    public int Identifier => 0;

    int IOutgoingPacket.Length => Variable.GetByteCount(Reason);

    public required string Reason { get; init; }

    void IOutgoingPacket.Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Reason);
    }
}