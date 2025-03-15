using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record JoinGamePacket(
    int Entity,
    GameMode GameMode,
    Dimension Dimension,
    Difficulty Difficulty,
    byte Players,
    WorldType Type,
    bool ReducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;
}