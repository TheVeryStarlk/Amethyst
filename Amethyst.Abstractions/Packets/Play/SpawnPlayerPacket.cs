using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Packets.Play;

// Metadata and current item are not implemented.
public sealed class SpawnPlayerPacket(IPlayer player) : IOutgoingPacket
{
    public int Identifier => 12;

    public IPlayer Player => player;
}