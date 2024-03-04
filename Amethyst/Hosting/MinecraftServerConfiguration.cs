using System.Net;
using Amethyst.Api.Components;

namespace Amethyst.Hosting;

public sealed class MinecraftServerConfiguration
{
    public IPEndPoint ListeningEndPoint { get; set; } = new IPEndPoint(IPAddress.Any, 25565);

    public ChatMessage Description { get; set; } = ChatMessage.Create("An Amethyst server");

    public int MaximumPlayerCount { get; set; } = 20;
}