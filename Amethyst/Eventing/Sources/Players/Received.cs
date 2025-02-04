using Amethyst.Entities;
using Amethyst.Protocol;

namespace Amethyst.Eventing.Sources.Players;

public sealed class Received(Packet packet) : Event<Player>
{
    public Packet Packet => packet;
}