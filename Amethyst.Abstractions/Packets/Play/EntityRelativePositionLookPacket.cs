using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityRelativePositionLookPacket(int unique, Position position, byte yaw, byte pitch, bool ground) : IOutgoingPacket
{
    public int Identifier => 23;

    public int Unique => unique;

    public Position Position => position;

    public byte Yaw => yaw;

    public byte Pitch => pitch;

    public bool Ground => ground;
}