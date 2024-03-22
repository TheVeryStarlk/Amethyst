using Amethyst.Api.Entities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class EntityLookAndRelativeMovePacket : IOutgoingPacket
{
    public int Identifier => 0x17;

    public required IEntity Entity { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Entity.Identifier);
        writer.WriteByte((byte) ((Entity.Position.X - Entity.OldPosition.X) * 32.0D));
        writer.WriteByte((byte) ((Entity.Position.Y - Entity.OldPosition.Y) * 32.0D));
        writer.WriteByte((byte) ((Entity.Position.Z - Entity.OldPosition.Z) * 32.0D));
        writer.WriteByte((byte) (Entity.Yaw / 256));
        writer.WriteByte((byte) (Entity.Pitch / 256));
        writer.WriteBoolean(Entity.OnGround);
    }
}