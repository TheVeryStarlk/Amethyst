namespace Amethyst.Abstractions.Packets.Play;

/// <summary>
/// Changes the player's selected hotbar slot.
/// </summary>
public sealed class HeldSlotPacket(byte slot) : IOutgoingPacket
{
    public int Identifier => 9;

    /// <remarks>Ranges from 0 to 8.</remarks>
    public byte Slot => slot;
}
