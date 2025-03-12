using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities.Players;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public IWorld World { get; }

    // Could be a string - where we get it from Minecraft's account services.
    public Guid Guid { get; }

    public GameMode GameMode { get; }

    public string Username { get; }

    public string? Locale { get; }

    public byte ViewDistance { get; }

    public void Send(Message message, MessagePosition position = MessagePosition.Box);

    public void Disconnect(Message message);
}