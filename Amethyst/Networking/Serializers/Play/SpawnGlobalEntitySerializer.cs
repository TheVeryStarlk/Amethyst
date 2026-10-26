using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class SpawnGlobalEntitySerializer([Variable] int unique, byte type, int x, int y, int z) : ISerializer<SpawnGlobalEntityPacket, SpawnGlobalEntitySerializer>
{
    public static SpawnGlobalEntitySerializer Create(SpawnGlobalEntityPacket packet)
    {
        // Lightning is the only global entity, positions are fixed-point with 5 fractional bits.
        return new SpawnGlobalEntitySerializer(
            packet.Lightning.Unique,
            1,
            (int) (packet.Lightning.Position.X * 32D),
            (int) (packet.Lightning.Position.Y * 32D),
            (int) (packet.Lightning.Position.Z * 32D));
    }
}