using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record EntityHeadLook(IEntity Entity) : IOutgoingPacket
{
    public int Identifier => 25;

    public int Length => Variable.GetByteCount(Entity.Identifier) + sizeof(byte);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableInteger(Entity.Identifier).WriteVariableInteger((byte) (Entity.Yaw % 360 / 360 * 256));
    }
}