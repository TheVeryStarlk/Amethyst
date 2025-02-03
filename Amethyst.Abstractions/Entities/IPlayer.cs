using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Entities;

public interface IPlayer : IEntity
{
    public IClient Client { get; }

    public string Username { get; }

    public ValueTask SendAsync(Message message, MessagePosition position);

    public ValueTask KeepAliveAsync();

    public void Disconnect(Message reason);
}