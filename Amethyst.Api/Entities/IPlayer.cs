namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Task KickAsync();
}