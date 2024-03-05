using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api;

public interface IMinecraftServer : IAsyncDisposable
{
    public ServerStatus Status { get; }

    public Task KickPlayer(IPlayer player, ChatMessage reason);
}