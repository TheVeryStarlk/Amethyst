using Amethyst.Api.Components;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Guid Guid { get; }

    public IServer Server { get; }

    public string Username { get; }

    public GameMode GameMode { get; set; }

    public Task SendChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box);

    public Task DisconnectAsync(ChatMessage reason);

    public Task SpawnPlayerAsync(IPlayer player);

    public Task DestroyEntitiesAsync(params IEntity[] entities);
}

public enum GameMode
{
    Survival,
    Creative,
    Adventure,
    Spectator
}