using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnPaintingSerializer(int unique, string motive, Position position, byte direction) : ISerializer<SpawnPaintingPacket, SpawnPaintingSerializer>
{
    public int Length => Variable.GetByteCount(unique) + Variable.GetByteCount(motive) + sizeof(long) + sizeof(byte);

    public static SpawnPaintingSerializer Create(SpawnPaintingPacket packet)
    {
        return new SpawnPaintingSerializer(
            packet.Painting.Unique,
            packet.Painting.Motive.ToString(),
            packet.Painting.Position,
            (byte) packet.Painting.Direction);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteVariableString(motive)
            .WritePosition(position)
            .WriteByte(direction);
    }
}