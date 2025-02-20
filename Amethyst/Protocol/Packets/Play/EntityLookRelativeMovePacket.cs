using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

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
        Console.WriteLine((byte) (Entity.Yaw / 256));

        SpanWriter
            .Create(span)
            .WriteVariableInteger(Entity.Identifier)
            .WriteByte((byte) Relative.X)
            .WriteByte((byte) Relative.Y)
            .WriteByte((byte) Relative.Z)
            .WriteByte((byte) (Entity.Yaw / 256))
            .WriteByte((byte) (Entity.Pitch / 256))
            .WriteBoolean(Entity.OnGround);
    }
}