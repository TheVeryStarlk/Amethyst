using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class RespawnPacket(IPlayer player, IWorld world) : IOutgoingPacket
{
    public int Identifier => 7;

    public IPlayer Player => player;

    public IWorld World => world;
}