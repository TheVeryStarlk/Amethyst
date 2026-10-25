using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Worlds;

public interface IWorld
{
    public string Name { get; }

    public WorldType Type { get; }

    public Dimension Dimension { get; }

    public Difficulty Difficulty { get; }

    public IGenerator Generator { get; }

    /// <summary>
    /// The players in this world, keyed by their username ignoring case.
    /// </summary>
    public IReadOnlyDictionary<string, IPlayer> Players { get; }

    /// <summary>
    /// The entities in this world other than players, keyed by their unique.
    /// </summary>
    public IReadOnlyDictionary<int, ISpawnable> Entities { get; }

    public Block this[Position position] { get; set; }

    public Block this[int x, int y, int z] { get; set; }

    /// <summary>
    /// Returns a <see cref="IChunk"/>.
    /// </summary>
    /// <param name="x">The <see cref="IChunk"/>'s X position.</param>
    /// <param name="z">The <see cref="IChunk"/>'s Z position.</param>
    public IChunk this[int x, int z] { get; }

    /// <summary>
    /// Creates an entity tracked by this world, without showing it to players.
    /// </summary>
    /// <typeparam name="T">The entity's interface, like <see cref="Entities.Mobs.IZombie"/>.</typeparam>
    /// <param name="position">The entity's position.</param>
    /// <returns>The newly created entity.</returns>
    public T Create<T>(Position position) where T : ISpawnable;

    /// <summary>
    /// Stops this world from tracking the entity, without hiding it from players.
    /// </summary>
    public void Remove(ISpawnable entity);
}