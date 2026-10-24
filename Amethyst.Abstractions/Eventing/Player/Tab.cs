using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Tab(string behind) : IEvent<IPlayer>
{
    public string Behind => behind;

    /// <remarks>
    /// Each match replaces the last word of <see cref="Behind"/>.
    /// When completing a command's name, matches are written without the leading slash.
    /// </remarks>
    public string[] Matches { get; set; } = [];
}