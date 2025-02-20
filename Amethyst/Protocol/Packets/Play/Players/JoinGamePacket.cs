using Amethyst.Abstractions.Protocol;

namespace Amethyst.Protocol.Packets.Play.Players;

public sealed record JoinGamePacket(int Entity, byte GameMode, sbyte Dimension, byte Difficulty, byte Players, string Type, bool ReducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;

    public int Length => sizeof(int)
                         + sizeof(byte)
                         + sizeof(sbyte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + Variable.GetByteCount(Type)
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
            .WriteVariableString(Type)
            .WriteBoolean(ReducedDebugInformation);
    }
}