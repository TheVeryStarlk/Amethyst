using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class PositionLookPacket(Position position, float yaw, float pitch) : IOutgoingPacket
{
    public int Identifier => 8;

    public Position Position => position;

    public float Yaw => yaw;

    public float Pitch => pitch;
}