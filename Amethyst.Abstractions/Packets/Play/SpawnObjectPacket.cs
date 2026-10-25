using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

/// <summary>
/// Spawns an entity that is not a mob, painting, experience orb or lightning, without its metadata.
/// </summary>
public sealed class SpawnObjectPacket(ISpawnable entity) : IOutgoingPacket
{
    public int Identifier => 14;

    public ISpawnable Entity => entity;
}