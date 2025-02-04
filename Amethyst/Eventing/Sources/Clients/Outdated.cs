using Amethyst.Components.Messages;

namespace Amethyst.Eventing.Sources.Clients;

public sealed class Outdated(int version) : Event<Client>
{
    public int Version { get; init; } = version;

    public Message Message { get; set; } = "Outdated version. I'm still on 1.8!";
}