using Amethyst.Api.Components;
using Amethyst.Api.Worlds;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Guid Guid { get; }

    public string Username { get; }

    public GameMode GameMode { get; set; }

    public byte ViewDistance { get; set; }

    public Task SendChatAsync(Chat chat, ChatPosition position = ChatPosition.Box);

    public Task KickAsync();
}

public enum GameMode
{
    Survival,
    Creative,
    Adventure,
    Spectator
}