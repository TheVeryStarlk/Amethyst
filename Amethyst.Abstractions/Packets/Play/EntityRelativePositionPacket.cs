using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

// Create the other look and position versions as well.
public sealed class EntityRelativePositionPacket(int unique, Position position, bool ground) : IOutgoingPacket
{
    public int Identifier => 21;

    public int Unique => unique;

    public Position Position => position;

    public bool Ground => ground;
}