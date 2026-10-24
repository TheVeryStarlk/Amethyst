using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityHeadLookSerializer(int unique, byte yaw) : ISerializer<EntityHeadLookPacket, EntityHeadLookSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(byte);

    public static EntityHeadLookSerializer Create(EntityHeadLookPacket packet)
    {
        // Angles are in 1/256 of a full turn, going through an integer first so negative ones wrap instead of saturating.
        return new EntityHeadLookSerializer(packet.Entity.Unique, (byte) (int) (packet.Entity.Yaw / 360F * 256F));
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteByte(yaw);
    }
}