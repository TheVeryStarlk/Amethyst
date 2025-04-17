using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class JoinGamePacket(
    int entity,
    GameMode gameMode,
    Dimension dimension,
    Difficulty difficulty,
    byte players,
    WorldType type,
    bool reducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;

    public int Entity => entity;

    public GameMode GameMode => gameMode;

    public Dimension Dimension => dimension;

    public Difficulty Difficulty => difficulty;

    public byte Players => players;

    public WorldType Type => type;

    public bool ReducedDebugInformation => reducedDebugInformation;
}