using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityMetadataPacket(ISpawnable entity) : IOutgoingPacket
{
    public int Identifier => 28;

    public ISpawnable Entity => entity;
}