using Amethyst.Api.Components;

namespace Amethyst.Hosting;

public sealed class AmethystServerOptions
{
    public ushort ListeningPort { get; set; } = 25565;

    public ChatMessage Description { get; set; } = ChatMessage.Create("An Amethyst server");

    public int MaximumPlayerCount { get; set; } = 20;
}