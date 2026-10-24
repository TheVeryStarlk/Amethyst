using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityTeleportSerializer(int unique, int x, int y, int z, byte yaw, byte pitch, bool ground) : ISerializer<EntityTeleportPacket, EntityTeleportSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(bool);

    public static EntityTeleportSerializer Create(EntityTeleportPacket packet)
    {
        // Positions are fixed-point with 5 fractional bits, angles are in 1/256 of a full turn.
        // Angles go through an integer first so negative ones wrap instead of saturating.
        return new EntityTeleportSerializer(
            packet.Entity.Unique,
            (int) (packet.Entity.Position.X * 32D),
            (int) (packet.Entity.Position.Y * 32D),
            (int) (packet.Entity.Position.Z * 32D),
            (byte) (int) (packet.Entity.Yaw / 360F * 256F),
            (byte) (int) (packet.Entity.Pitch / 360F * 256F),
            packet.Entity.Ground);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z)
            .WriteByte(yaw)
            .WriteByte(pitch)
            .WriteBoolean(ground);
    }
}