using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

/// <summary>
/// The player selected a different hotbar slot.
/// </summary>
public sealed class Selected(short slot) : IEvent<IPlayer>
{
    /// <remarks>
    /// Ranges from 0 to 8.
    /// </remarks>
    public short Slot => slot;
}