using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play.Entities;

public sealed record EntityLookRelativeMovePacket(IEntity Entity, Location Relative) : IOutgoingPacket
{
    public int Identifier => 23;

    public int Length => Variable.GetByteCount(Entity.Identifier)
                         + sizeof(byte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + sizeof(bool);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableInteger(Entity.Identifier)
            .WriteByte((byte) Relative.X)
            .WriteByte((byte) Relative.Y)
            .WriteByte((byte) Relative.Z)
            .WriteByte(Entity.Yaw.ToAbsolute())
            .WriteByte(Entity.Pitch.ToAbsolute())
            .WriteBoolean(Entity.OnGround);
    }
}