using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

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