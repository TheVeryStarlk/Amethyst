using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class BlockPacket(Position position, Block block) : IOutgoingPacket
{
    public int Identifier => 35;

    public Position Position => position;

    public Block Block => block;
}