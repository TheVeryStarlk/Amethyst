using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record EntityHeadLook(IEntity Entity) : IOutgoingPacket
{
    public int Identifier => 25;

    public int Length => Variable.GetByteCount(Entity.Identifier) + sizeof(byte);

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableInteger(Entity.Identifier).WriteByte(Entity.Yaw.ToAbsolute());
    }
}