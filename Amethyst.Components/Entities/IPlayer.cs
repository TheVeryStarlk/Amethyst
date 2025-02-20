using Amethyst.Components.Messages;
using Amethyst.Components.Worlds;

namespace Amethyst.Components.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public Guid Guid { get; }

    public string Username { get; }

    public IWorld World { get; }

    public string? Locale { get; }

    public byte ViewDistance { get; }

    public void Teleport(Location location);

    public void Send(Message message, MessagePosition position = MessagePosition.Box);

    public void Disconnect(Message message);
}