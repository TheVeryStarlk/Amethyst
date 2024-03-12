using Amethyst.Api.Components;

namespace Amethyst.Api;

public sealed class ServerOptions
{
    public ushort ListeningPort { get; set; } = 25565;

    public ChatMessage Description { get; set; } = ChatMessage.Create("An Amethyst server");

    public int MaximumPlayerCount { get; set; } = 20;
}