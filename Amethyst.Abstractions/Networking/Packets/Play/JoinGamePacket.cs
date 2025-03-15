using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class JoinGamePacket(int entity, GameMode gameMode, Dimension dimension, Difficulty difficulty, byte players, WorldType type, bool reducedDebugInformation) : IOutgoingPacket
{
    public int Identifier => 1;

    public int Length => sizeof(int) + sizeof(byte) + sizeof(sbyte) + sizeof(byte) + sizeof(byte) + Variable.GetByteCount(type) + sizeof(bool);

    private readonly string type = type.ToString();

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(entity)
            .WriteByte((byte) gameMode)
            .WriteByte((byte) dimension)
            .WriteByte((byte) difficulty)
            .WriteByte(players)
            .WriteVariableString(type)
            .WriteBoolean(reducedDebugInformation);
    }
}