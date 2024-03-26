using Amethyst.Api.Components;
using Amethyst.Api.Levels;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Guid Guid { get; }

    public IServer Server { get; }

    public string Username { get; }

    public GameMode GameMode { get; set; }

    public List<Position> Chunks { get; set; }

    public Task SendChatMessageAsync(ChatMessage message, ChatMessagePosition messagePosition = ChatMessagePosition.Box);

    public Task DisconnectAsync(ChatMessage reason);

    public Task SpawnPlayerAsync(IPlayer player);

    public Task DestroyEntitiesAsync(params IEntity[] entities);

    Task UpdateEntitiesAsync(params IEntity[] where);
}

public enum GameMode
{
    Survival,
    Creative,
    Adventure,
    Spectator
}