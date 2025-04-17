using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class TabResponseSerializer(string[] matches) : ISerializer<TabResponsePacket, TabResponseSerializer>
{
    public int Length => Variable.GetByteCount(matches.Length) + matches.Sum(Variable.GetByteCount);

    public static TabResponseSerializer Create(TabResponsePacket packet)
    {
        return new TabResponseSerializer(packet.Matches);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter.Create(span).WriteVariableInteger(matches.Length);

        foreach (var match in matches)
        {
            writer.WriteVariableString(match);
        }
    }
}