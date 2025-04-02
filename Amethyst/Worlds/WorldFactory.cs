using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

/// <summary>
/// Provides support for creating <see cref="IWorld"/> instances.
/// </summary>
internal sealed class WorldFactory : IWorldFactory
{
    /// <summary>
    /// Creates a new <see cref="IWorld"/> instance.
    /// </summary>
    /// <param name="name">The <see cref="IWorld"/>'s name.</param>
    /// <param name="type">The <see cref="IWorld"/>'s type.</param>
    /// <param name="dimension">The <see cref="IWorld"/>'s dimension.</param>
    /// <param name="difficulty">The <see cref="IWorld"/>'s difficulty.</param>
    /// <returns>The newly created <see cref="IWorld"/> instance.</returns>
    /// <remarks>
    /// Uses an <see cref="EmptyGenerator"/> by default.
    /// </remarks>
    public IWorld Create(string name, WorldType type = WorldType.Default, Dimension dimension = Dimension.OverWorld, Difficulty difficulty = Difficulty.Peaceful)
    {
        return new World(name, type, dimension, difficulty, EmptyGenerator.Instance);
    }

    /// <summary>
    /// Creates a new <see cref="IWorld"/> instance.
    /// </summary>
    /// <param name="name">The <see cref="IWorld"/>'s name.</param>
    /// <param name="type">The <see cref="IWorld"/>'s type.</param>
    /// <param name="dimension">The <see cref="IWorld"/>'s dimension.</param>
    /// <param name="difficulty">The <see cref="IWorld"/>'s difficulty.</param>
    /// <param name="generator">The <see cref="IWorld"/>'s generator.</param>
    /// <returns>The newly created <see cref="IWorld"/> instance.</returns>
    public IWorld Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator)
    {
        return new World(name, type, dimension, difficulty, generator);
    }
}