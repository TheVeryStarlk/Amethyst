using System.Numerics;
using Amethyst.Api.Components;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public IMinecraftServer Server { get; }

    public Vector3 Position { get; set; }

    public Vector2 Rotation { get; set; }

    public GameMode GameMode { get; set; }

    public Task SendChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box);

    public Task DisconnectAsync(ChatMessage reason);
}

public enum GameMode
{
    Survival,
    Creative,
    Adventure,
    Spectator
}