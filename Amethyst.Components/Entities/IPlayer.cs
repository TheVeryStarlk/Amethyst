using Amethyst.Components.Messages;

namespace Amethyst.Components.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public string Username { get; }

    public float Yaw { get; }

    public float Pitch { get; }

    public bool OnGround { get; }

    public void Disconnect(Message message);
}