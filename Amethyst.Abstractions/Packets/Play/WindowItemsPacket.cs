using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

/// <summary>
/// Replaces every slot in the player's inventory window.
/// </summary>
public sealed class WindowItemsPacket(Item[] items) : IOutgoingPacket
{
    public int Identifier => 48;

    public Item[] Items => items;
}
