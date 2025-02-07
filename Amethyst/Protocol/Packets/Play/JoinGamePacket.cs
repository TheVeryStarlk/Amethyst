using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record JoinGamePacket(int Entity, byte GameMode, sbyte Dimension, byte Difficulty, byte Players, string LevelType, bool ReducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;

    public int Length => sizeof(int)
                         + sizeof(byte)
                         + sizeof(sbyte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + Variable.GetByteCount(LevelType)
                         + sizeof(bool);

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(Entity)
            .WriteByte(GameMode)
            .WriteByte((byte) Dimension)
            .WriteByte(Difficulty)
            .WriteByte(Players)
            .WriteVariableString(LevelType)
            .WriteBoolean(ReducedDebugInformation);
    }
}