using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class EntityRelativePositionSerializer(int unique, Position position, bool ground) : ISerializer<EntityRelativePositionPacket, EntityRelativePositionSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(byte) * 3 + sizeof(bool);

    public static EntityRelativePositionSerializer Create(EntityRelativePositionPacket packet)
    {
        return new EntityRelativePositionSerializer(packet.Unique, packet.Position, packet.Ground);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteFixedByte(position.X)
            .WriteFixedByte(position.Y)
            .WriteFixedByte(position.Z)
            .WriteBoolean(ground);
    }
}