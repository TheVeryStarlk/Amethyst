using Amethyst.Components.Messages;

namespace Amethyst.Components.Eventing.Sources.Clients;

public sealed class Outdated(int version) : Event<IClient>
{
    public int Version { get; init; } = version;

    public Message Message { get; set; } = "Outdated version.";
}