using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record JoinGamePacket(int Entity, GameMode GameMode, Dimension Dimension, Difficulty Difficulty, byte Players, WorldType Type, bool ReducedDebugInformation) : IOutgoingPacket
{
    public int Length => sizeof(int) + sizeof(byte) + sizeof(sbyte) + sizeof(byte) + sizeof(byte) + Variable.GetByteCount(type) + sizeof(bool);

    private readonly string type = Type.ToString();

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(Entity)
            .WriteByte((byte) GameMode)
            .WriteByte((byte) Dimension)
            .WriteByte((byte) Difficulty)
            .WriteByte(Players)
            .WriteVariableString(type)
            .WriteBoolean(ReducedDebugInformation);
    }
}