using Amethyst.Protocol;

namespace Amethyst.Eventing.Sources.Clients;

public sealed class Received(Packet packet) : Event<Client>
{
    public Packet Packet => packet;
}