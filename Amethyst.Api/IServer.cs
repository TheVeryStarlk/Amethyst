using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Api;

public interface IServer : IAsyncDisposable
{
    public static abstract int ProtocolVersion { get; }

    public ServerConfiguration Configuration { get; }

    public IEnumerable<IPlayer> Players { get; }

    public ChatMessage Description { get; set; }

    public Task BroadcastChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box);

    public Task DisconnectPlayerAsync(IPlayer player, ChatMessage reason);
}