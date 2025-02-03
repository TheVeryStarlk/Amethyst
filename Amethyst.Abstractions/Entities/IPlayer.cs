using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public string Username { get; }

    public ValueTask SendAsync(Message message, byte position);

    public void Disconnect(Message reason);
}