using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnPlayerSerializer(int unique, Guid guid, int x, int y, int z, byte yaw, byte pitch) : ISerializer<SpawnPlayerPacket, SpawnPlayerSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(long) * 2 + sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(short) + sizeof(byte);

    public static SpawnPlayerSerializer Create(SpawnPlayerPacket packet)
    {
        // Positions are fixed-point with 5 fractional bits, angles are in 1/256 of a full turn.
        // Angles go through an integer first so negative ones wrap instead of saturating.
        return new SpawnPlayerSerializer(
            packet.Player.Unique,
            packet.Player.Guid,
            (int) (packet.Player.Position.X * 32D),
            (int) (packet.Player.Position.Y * 32D),
            (int) (packet.Player.Position.Z * 32D),
            (byte) (int) (packet.Player.Yaw / 360F * 256F),
            (byte) (int) (packet.Player.Pitch / 360F * 256F));
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteGuid(guid)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z)
            .WriteByte(yaw)
            .WriteByte(pitch)
            .WriteShort(0)
            .WriteByte(127);
    }
}