using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class SpawnMobPacket(IMob mob) : IOutgoingPacket
{
    public int Identifier => 15;

    public IMob Mob => mob;
}