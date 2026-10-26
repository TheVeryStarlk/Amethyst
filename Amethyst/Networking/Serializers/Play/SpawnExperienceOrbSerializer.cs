using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnExperienceOrbSerializer(int unique, int x, int y, int z, short count) : ISerializer<SpawnExperienceOrbPacket, SpawnExperienceOrbSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(int) * 3 + sizeof(short);

    public static SpawnExperienceOrbSerializer Create(SpawnExperienceOrbPacket packet)
    {
        // Positions are fixed-point with 5 fractional bits.
        return new SpawnExperienceOrbSerializer(
            packet.Orb.Unique,
            (int) (packet.Orb.Position.X * 32D),
            (int) (packet.Orb.Position.Y * 32D),
            (int) (packet.Orb.Position.Z * 32D),
            packet.Orb.Count);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z)
            .WriteShort(count);
    }
}