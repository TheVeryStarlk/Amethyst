using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Worlds;

/// <summary>
/// Provides support for creating <see cref="IWorld"/> instances.
/// </summary>
public interface IWorldFactory
{
    /// <summary>
    /// Every <see cref="IWorld"/> created by this factory.
    /// </summary>
    public IReadOnlyCollection<IWorld> Worlds { get; }

    /// <summary>
    /// Every player in any <see cref="IWorld"/>, keyed by their username ignoring case.
    /// </summary>
    public IReadOnlyDictionary<string, IPlayer> Players { get; }

    /// <summary>
    /// Creates a new <see cref="IWorld"/> instance.
    /// </summary>
    /// <param name="name">The <see cref="IWorld"/>'s name.</param>
    /// <param name="generator">The <see cref="IWorld"/>'s generator.</param>
    /// <param name="type">The <see cref="IWorld"/>'s type.</param>
    /// <param name="dimension">The <see cref="IWorld"/>'s dimension.</param>
    /// <param name="difficulty">The <see cref="IWorld"/>'s difficulty.</param>
    /// <returns>The newly created <see cref="IWorld"/> instance.</returns>
    public IWorld Create(string name, IGenerator generator, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful);
}