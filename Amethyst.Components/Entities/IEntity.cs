namespace Amethyst.Components.Entities;

public interface IEntity
{
    public int Identifier { get; }

    public Location Location { get; }
}