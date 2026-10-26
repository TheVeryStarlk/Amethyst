using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnGlobalEntitySerializer(int unique, int x, int y, int z) : ISerializer<SpawnGlobalEntityPacket, SpawnGlobalEntitySerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(byte) + sizeof(int) * 3;

    public static SpawnGlobalEntitySerializer Create(SpawnGlobalEntityPacket packet)
    {
        // Positions are fixed-point with 5 fractional bits.
        return new SpawnGlobalEntitySerializer(
            packet.Lightning.Unique,
            (int) (packet.Lightning.Position.X * 32D),
            (int) (packet.Lightning.Position.Y * 32D),
            (int) (packet.Lightning.Position.Z * 32D));
    }

    public void Write(Span<byte> span)
    {
        // Lightning is the only global entity.
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteByte(1)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z);
    }
}