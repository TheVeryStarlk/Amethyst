using Amethyst.Abstractions.Entities.Objects;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class SpawnExperienceOrbPacket(IExperienceOrb orb) : IOutgoingPacket
{
    public int Identifier => 17;

    public IExperienceOrb Orb => orb;
}