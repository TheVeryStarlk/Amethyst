using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Eventing.Sources.Client;

public sealed class Outdated(int version) : Event<IClient>
{
    public int Version { get; init; } = version;

    public Message Message { get; set; } = "Outdated version. I'm still on 1.8.";
}