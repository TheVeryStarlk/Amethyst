using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class DestroyEntitiesSerializer(int[] uniques) : ISerializer<DestroyEntitiesPacket, DestroyEntitiesSerializer>
{
    public int Length => Variable.GetByteCount(uniques.Length) + uniques.Sum(Variable.GetByteCount);

    public static DestroyEntitiesSerializer Create(DestroyEntitiesPacket packet)
    {
        return new DestroyEntitiesSerializer(packet.Entities.Select(entity => entity.Unique).ToArray());
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter.Create(span).WriteVariableInteger(uniques.Length);

        foreach (var unique in uniques)
        {
            writer.WriteVariableInteger(unique);
        }
    }
}