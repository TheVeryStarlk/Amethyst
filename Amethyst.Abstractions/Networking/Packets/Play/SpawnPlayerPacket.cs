using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Networking.Packets.Play;

// Metadata and current item are not implemented.
public sealed class SpawnPlayerPacket(int unique, Guid guid, Position position, byte yaw, byte pitch) : IOutgoingPacket
{
    public int Identifier => 12;

    public int Unique => unique;

    public Guid Guid => guid;

    public Position Position => position;

    public byte Yaw => yaw;

    public byte Pitch => pitch;
}