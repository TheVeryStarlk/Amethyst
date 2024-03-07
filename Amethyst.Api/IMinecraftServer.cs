using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api;

public interface IMinecraftServer : IAsyncDisposable
{
    public ServerStatus Status { get; }

    public IEnumerable<IPlayer> Players { get; }

    public Task BroadcastChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box);

    public Task DisconnectPlayerAsync(IPlayer player, ChatMessage reason);
}