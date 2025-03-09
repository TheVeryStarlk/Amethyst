using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Clients;

public sealed class Outdated(int version) : Event<IClient>
{
    public int Version { get; init; } = version;

    public Message Message { get; set; } = "Outdated version.";
}