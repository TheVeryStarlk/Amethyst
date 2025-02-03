namespace Amethyst.Protocol.Packets.Play;

internal sealed record JoinGamePacket(
    int Entity,
    byte GameMode,
    sbyte Dimension,
    byte Difficulty,
    byte MaximumPlayerCount,
    string LevelType,
    bool ReducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;

    public int Length => sizeof(int)
                         + sizeof(byte)
                         + sizeof(sbyte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + Variable.GetByteCount(LevelType)
                         + sizeof(bool);

    public void Write(ref SpanWriter writer)
    {
        writer.WriteInteger(Entity);
        writer.WriteByte(GameMode);
        writer.WriteByte((byte) Dimension);
        writer.WriteByte(Difficulty);
        writer.WriteByte(MaximumPlayerCount);
        writer.WriteVariableString(LevelType);
        writer.WriteBoolean(ReducedDebugInformation);
    }
}