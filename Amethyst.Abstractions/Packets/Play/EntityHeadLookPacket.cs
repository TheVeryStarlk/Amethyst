using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityHeadLookPacket(IEntity entity) : IOutgoingPacket
{
    public int Identifier => 25;

    public IEntity Entity => entity;
}