using Amethyst.Api.Components;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Guid Guid { get; }

    public string Username { get; }

    public GameMode GameMode { get; set; }

    public Task KickAsync();
}

public enum GameMode
{
    Survival,
    Creative,
    Adventure,
    Spectator
}