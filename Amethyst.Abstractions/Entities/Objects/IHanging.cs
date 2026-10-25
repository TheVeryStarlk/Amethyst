namespace Amethyst.Abstractions.Entities.Objects;

/// <remarks>Its position is the block it hangs in, clients only read it when the entity is spawned.</remarks>
public interface IHanging : ISpawnable
{
    /// <summary>
    /// The direction the entity faces, clients only read it when the entity is spawned.
    /// </summary>
    public Direction Direction { get; set; }
}

public enum Direction
{
    South,
    West,
    North,
    East
}