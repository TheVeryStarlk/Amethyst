using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

/// <summary>
/// Sets the velocity of an entity, players receiving their own velocity get knocked back by it.
/// </summary>
public sealed class EntityVelocityPacket(IEntity entity, Velocity velocity) : IOutgoingPacket
{
    public int Identifier => 18;

    public IEntity Entity => entity;

    public Velocity Velocity => velocity;
}