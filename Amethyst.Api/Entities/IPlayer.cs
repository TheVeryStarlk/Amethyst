using Amethyst.Api.Components;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Task DisconnectAsync(ChatMessage reason);
}