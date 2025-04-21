using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityRelativePositionLookSerializer(int unique, Position position, byte yaw, byte pitch, bool ground) : ISerializer<EntityRelativePositionLookPacket, EntityRelativePositionLookSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(byte) * 3 + sizeof(byte) * 2 + sizeof(bool);

    public static EntityRelativePositionLookSerializer Create(EntityRelativePositionLookPacket lookPacket)
    {
        return new EntityRelativePositionLookSerializer(lookPacket.Unique, lookPacket.Position, lookPacket.Yaw, lookPacket.Pitch, lookPacket.Ground);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteFixedByte(position.X)
            .WriteFixedByte(position.Y)
            .WriteFixedByte(position.Z)
            .WriteByte(yaw)
            .WriteByte(pitch)
            .WriteBoolean(ground);
    }
}