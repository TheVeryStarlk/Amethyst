using Amethyst.Api.Components;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Guid Guid { get; }

    public string Username { get; }

    public GameMode GameMode { get; set; }

    public byte ViewDistance { get; set; }

    public void SendChat(Chat chat, ChatPosition position = ChatPosition.Box);

    public void Disconnect(Chat reason);
}

public enum GameMode
{
    Survival,
    Creative,
    Adventure,
    Spectator
}