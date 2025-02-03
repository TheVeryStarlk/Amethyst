namespace Amethyst.Abstractions.Entities;

public interface IEntity
{
    public IServer Server { get; }

    public int Identifier { get; }
}