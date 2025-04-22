using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class JoinGamePacket(IPlayer player, IWorld world, byte list, bool reducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;

    public IPlayer Player => player;

    public IWorld World => world;

    public byte List => list;

    public bool ReducedDebugInformation => reducedDebugInformation;
}