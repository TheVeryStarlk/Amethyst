using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class StatisticSerializer(IDictionary<string, int> values) : ISerializer<StatisticsPacket, StatisticSerializer>
{
    public int Length => Variable.GetByteCount(values.Count) + values.Sum(pair => Variable.GetByteCount(pair.Key)) + values.Sum(pair => Variable.GetByteCount(pair.Value));

    public static StatisticSerializer Create(StatisticsPacket packet)
    {
        return new StatisticSerializer(packet.Values);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter.Create(span).WriteVariableInteger(values.Count);

        foreach (var pair in values)
        {
            writer.WriteVariableString(pair.Key);
            writer.WriteVariableInteger(pair.Value);
        }
    }
}