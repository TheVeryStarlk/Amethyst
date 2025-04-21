using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityRelativePositionPacket(int unique, Position position, bool ground) : IOutgoingPacket
{
    public int Identifier => 21;

    public int Unique => unique;

    public Position Position => position;

    public bool Ground => ground;
}