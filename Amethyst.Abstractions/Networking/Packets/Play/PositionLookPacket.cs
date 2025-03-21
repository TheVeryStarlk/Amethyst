using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class PositionLookPacket(Location location, float yaw, float pitch) : IOutgoingPacket
{
    public int Identifier => 8;

    public Location Location => location;

    public float Yaw => yaw;

    public float Pitch => pitch;
}