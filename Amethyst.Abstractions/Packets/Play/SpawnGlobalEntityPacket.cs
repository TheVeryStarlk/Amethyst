using Amethyst.Abstractions.Entities.Objects;

namespace Amethyst.Abstractions.Packets.Play;

/// <summary>
/// Strikes lightning, which every player sees no matter how far away it is.
/// </summary>
public sealed class SpawnGlobalEntityPacket(ILightning lightning) : IOutgoingPacket
{
    public int Identifier => 44;

    public ILightning Lightning => lightning;
}