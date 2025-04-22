using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class DestroyEntitiesPacket(params IEntity[] entities) : IOutgoingPacket
{
    public int Identifier => 19;

    public IEntity[] Entities => entities;
}