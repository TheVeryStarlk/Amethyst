using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record PositionLookPacket(Location Location, float Yaw, float Pitch, bool OnGround) : IOutgoingPacket;