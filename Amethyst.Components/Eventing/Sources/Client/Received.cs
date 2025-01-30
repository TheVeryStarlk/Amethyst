using Amethyst.Protocol;

namespace Amethyst.Components.Eventing.Sources.Client;

public sealed class Received(Packet packet) : Event<IClient>
{
    public Packet Packet { get; init; } = packet;
}