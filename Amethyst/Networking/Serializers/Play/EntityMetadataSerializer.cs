using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityMetadataSerializer(int unique, Metadata metadata) : ISerializer<EntityMetadataPacket, EntityMetadataSerializer>
{
    public int Length => Variable.GetByteCount(unique) + metadata.Length;

    public static EntityMetadataSerializer Create(EntityMetadataPacket packet)
    {
        return new EntityMetadataSerializer(packet.Entity.Unique, Metadata.Create(packet.Entity));
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(unique);

        metadata.Write(ref writer);
    }
}