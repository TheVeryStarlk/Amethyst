using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EffectPacket(int effect, Position position, int metadata, bool isGlobal = false) : IOutgoingPacket
{
    public int Identifier => 40;

    public int Effect => effect;

    public Position Position => position;

    public int Metadata => metadata;

    /// <summary>
    /// Whether the effect is heard at the same volume no matter how far away it is.
    /// </summary>
    public bool IsGlobal => isGlobal;
}