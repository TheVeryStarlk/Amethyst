using Amethyst.Abstractions.Protocol;

namespace Amethyst.Abstractions.Eventing.Sources.Client;

public sealed class Received(Packet packet) : Event<IClient>
{
    public Packet Packet { get; init; } = packet;
}