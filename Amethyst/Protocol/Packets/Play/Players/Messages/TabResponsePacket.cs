using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play.Players.Messages;

public sealed record TabResponsePacket(string[] Matches) : IOutgoingPacket
{
    public int Identifier => 58;

    public int Length => Variable.GetByteCount(Matches.Length) + Matches.Sum(Variable.GetByteCount);

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter.Create(span).WriteVariableInteger(Matches.Length);

        foreach (var match in Matches)
        {
            writer.WriteVariableString(match);
        }
    }
}