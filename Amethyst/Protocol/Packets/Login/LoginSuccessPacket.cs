using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginSuccessPacket(string Guid, string Username) : IOutgoingPacket
{
    public int Identifier => 2;

    public int Length => Variable.GetByteCount(Guid) + Variable.GetByteCount(Username);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(Guid).WriteVariableString(Username);
    }
}