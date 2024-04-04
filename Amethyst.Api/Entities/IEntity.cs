namespace Amethyst.Api.Entities;

public interface IEntity
{
    public int Identifier { get; }

    public IServer Server { get; }
}