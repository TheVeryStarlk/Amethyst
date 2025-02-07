namespace Amethyst.Protocol.Packets.Play;

public abstract record JoinGamePacketBase(
    int Entity,
    byte GameMode,
    sbyte Dimension,
    byte Difficulty,
    byte Players,
    string LevelType,
    bool ReducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;
}