using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play.Entities;

public sealed record DestroyEntitiesPacket(params IEntity[] Entities) : IOutgoingPacket
{
    public int Identifier => 19;

    public int Length => Variable.GetByteCount(Entities.Length) + Entities.Length;

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter.Create(span).WriteVariableInteger(Entities.Length);

        foreach (var entity in Entities)
        {
            writer.WriteVariableInteger(entity.Identifier);
        }
    }
}