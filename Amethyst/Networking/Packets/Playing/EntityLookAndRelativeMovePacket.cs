using Amethyst.Api.Entities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class EntityLookAndRelativeMovePacket : IOutgoingPacket
{
    public int Identifier => 0x17;

    public required IEntity Entity { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Entity.Identifier);
        writer.WriteByte((byte) (Entity.DeltaPosition.X));
        writer.WriteByte((byte) (Entity.DeltaPosition.Y));
        writer.WriteByte((byte) (Entity.DeltaPosition.Z));
        writer.WriteByte((byte) (Entity.Yaw / 256));
        writer.WriteByte((byte) (Entity.Pitch / 256));
        writer.WriteBoolean(Entity.OnGround);
    }
}