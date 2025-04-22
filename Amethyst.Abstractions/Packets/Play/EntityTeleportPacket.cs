using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityTeleportPacket(IEntity entity) : IOutgoingPacket
{
    public int Identifier => 24;

    public IEntity Entity => entity;
}