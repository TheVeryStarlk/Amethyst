using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnPlayerSerializer(int unique, Guid guid, double x, double y, double z, byte yaw, byte pitch) : ISerializer<SpawnPlayerPacket, SpawnPlayerSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(long) * 2 + sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(short) + sizeof(byte);

    public static SpawnPlayerSerializer Create(SpawnPlayerPacket packet)
    {
        return new SpawnPlayerSerializer(
            packet.Unique,
            packet.Guid,
            packet.Position.X,
            packet.Position.Y,
            packet.Position.Z,
            packet.Yaw,
            packet.Pitch);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteGuid(guid)
            .WriteFixedDouble(x)
            .WriteFixedDouble(y)
            .WriteFixedDouble(z)
            .WriteByte(yaw)
            .WriteByte(pitch)
            .WriteShort(0)
            .WriteByte(127);
    }
}