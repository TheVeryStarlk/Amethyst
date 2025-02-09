using Amethyst.Components.Messages;

namespace Amethyst.Components.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public string Username { get; }

    public float Yaw { get; }

    public float Pitch { get; }

    public bool OnGround { get; }

    public void Move(double x, double y, double z, float pitch, float yaw);

    public void Send(Message message, MessagePosition position = MessagePosition.Box);

    public void Disconnect(Message message);
}