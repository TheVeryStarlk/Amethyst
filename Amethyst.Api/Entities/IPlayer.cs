using Amethyst.Api.Components;

namespace Amethyst.Api.Entities;

public interface IPlayer : IEntity
{
    public Task SendChatAsync(Chat chat);

    public Task KickAsync();
}