using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class SpawnPlayerSerializer(
    [Variable] int unique,
    Guid guid,
    int x,
    int y,
    int z,
    byte yaw,
    byte pitch,
    short item,
    byte metadata) : ISerializer<SpawnPlayerPacket, SpawnPlayerSerializer>
{
    public static SpawnPlayerSerializer Create(SpawnPlayerPacket packet)
    {
        // Positions are fixed-point with 5 fractional bits, angles are in 1/256 of a full turn.
        // Angles go through an integer first so negative ones wrap instead of saturating.
        // Players spawn holding nothing, with metadata that is only its end marker.
        return new SpawnPlayerSerializer(
            packet.Player.Unique,
            packet.Player.Guid,
            (int) (packet.Player.Position.X * 32D),
            (int) (packet.Player.Position.Y * 32D),
            (int) (packet.Player.Position.Z * 32D),
            (byte) (int) (packet.Player.Yaw / 360F * 256F),
            (byte) (int) (packet.Player.Pitch / 360F * 256F),
            0,
            127);
    }
}