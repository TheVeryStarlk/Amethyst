using Amethyst.Components.Messages;
using Amethyst.Components.Worlds;

namespace Amethyst.Components.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public string Username { get; }

    public IWorld World { get; }

    public float Yaw { get; }

    public float Pitch { get; }

    public bool OnGround { get; }

    public string? Locale { get; }

    public byte ViewDistance { get; }

    public void Teleport(Location location);

    public void Send(Message message, MessagePosition position = MessagePosition.Box);

    public void Disconnect(Message message);
}