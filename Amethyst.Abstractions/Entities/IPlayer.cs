using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Entities;

// Research a good way to implement the UUID version used in Minecraft.
public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public IWorld World { get; }

    public string Username { get; }

    public string? Locale { get; }

    public byte ViewDistance { get; }

    public void Send(Message message, MessagePosition position = MessagePosition.Box);

    public void Disconnect(Message message);
}