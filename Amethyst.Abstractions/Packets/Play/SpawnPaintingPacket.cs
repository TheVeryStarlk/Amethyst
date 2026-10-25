using Amethyst.Abstractions.Entities.Objects;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class SpawnPaintingPacket(IPainting painting) : IOutgoingPacket
{
    public int Identifier => 16;

    public IPainting Painting => painting;
}