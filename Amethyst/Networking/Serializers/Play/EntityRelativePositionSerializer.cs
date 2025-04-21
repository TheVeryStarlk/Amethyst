using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityRelativePositionSerializer(int unique, byte x, byte y, byte z, bool ground) : ISerializer<EntityRelativePositionPacket, EntityRelativePositionSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(byte) * 3 + sizeof(bool);

    public static EntityRelativePositionSerializer Create(EntityRelativePositionPacket packet)
    {
        return new EntityRelativePositionSerializer(
            packet.Unique,
            (byte) packet.Position.X,
            (byte) packet.Position.Y,
            (byte) packet.Position.Z,
            packet.Ground);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteFixedByte(x)
            .WriteFixedByte(y)
            .WriteFixedByte(z)
            .WriteBoolean(ground);
    }
}