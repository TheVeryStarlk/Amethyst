using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityVelocitySerializer(int unique, short x, short y, short z) : ISerializer<EntityVelocityPacket, EntityVelocitySerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(short) * 3;

    public static EntityVelocitySerializer Create(EntityVelocityPacket packet)
    {
        // Velocities are in 1/8000 of a block per tick, capped like clients do.
        return new EntityVelocitySerializer(
            packet.Entity.Unique,
            (short) (Math.Clamp(packet.Velocity.X, -3.9D, 3.9D) * 8000D),
            (short) (Math.Clamp(packet.Velocity.Y, -3.9D, 3.9D) * 8000D),
            (short) (Math.Clamp(packet.Velocity.Z, -3.9D, 3.9D) * 8000D));
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteShort(x)
            .WriteShort(y)
            .WriteShort(z);
    }
}