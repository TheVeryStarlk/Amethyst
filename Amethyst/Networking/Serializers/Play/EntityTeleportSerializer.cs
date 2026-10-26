using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class EntityTeleportSerializer([Variable] int unique, int x, int y, int z, byte yaw, byte pitch, bool ground) : ISerializer<EntityTeleportPacket, EntityTeleportSerializer>
{
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
}