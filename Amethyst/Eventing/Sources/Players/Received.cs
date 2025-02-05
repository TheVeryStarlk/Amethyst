using Amethyst.Entities;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets;

namespace Amethyst.Eventing.Sources.Players;

public sealed class Received(Packet packet) : Event<Player>
{
    public Packet Packet => packet;
}