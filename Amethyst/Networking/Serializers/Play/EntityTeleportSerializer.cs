using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityTeleportSerializer(int unique, double x, double y, double z, float yaw, float pitch, bool ground) : ISerializer<EntityTeleportPacket, EntityTeleportSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(bool);

    public static EntityTeleportSerializer Create(EntityTeleportPacket packet)
    {
        return new EntityTeleportSerializer(
            packet.Entity.Unique,
            packet.Entity.Position.X,
            packet.Entity.Position.Y,
            packet.Entity.Position.Z,
            packet.Entity.Yaw,
            packet.Entity.Pitch,
            packet.Entity.Ground);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteFixedDouble(x)
            .WriteFixedDouble(y)
            .WriteFixedDouble(z)
            .WriteAngle(yaw)
            .WriteAngle(pitch)
            .WriteBoolean(ground);
    }
}