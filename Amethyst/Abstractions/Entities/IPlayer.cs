using Amethyst.Components.Messages;

namespace Amethyst.Abstractions.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public string Username { get; }

    public ValueTask SendAsync(Message message, MessagePosition position);

    public void Disconnect(Message message);
}