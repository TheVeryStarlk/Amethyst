using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityRelativePositionLookPacket(int unique, Position position, float yaw, float pitch, bool ground) : IOutgoingPacket
{
    public int Identifier => 23;

    public int Unique => unique;

    public Position Position => position;

    public float Yaw => yaw;

    public float Pitch => pitch;

    public bool Ground => ground;
}