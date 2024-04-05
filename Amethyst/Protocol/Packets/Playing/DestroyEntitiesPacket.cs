using Amethyst.Api.Entities;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class DestroyEntitiesPacket : IOutgoingPacket
{
    public int Identifier => 0x13;

    public required IEntity[] Entities { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Entities.Length);

        foreach (var entity in Entities)
        {
            writer.WriteVariableInteger(entity.Identifier);
        }
    }
}