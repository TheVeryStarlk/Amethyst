using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class JoinGameSerializer(
    int entity,
    byte gameMode,
    byte dimension,
    byte difficulty,
    byte players,
    string type,
    bool reducedDebugInformation) : ISerializer<JoinGamePacket>
{
    public int Length => sizeof(int)
                         + sizeof(byte)
                         + sizeof(sbyte)
                         + sizeof(byte)
                         + sizeof(byte)
                         + Variable.GetByteCount(type)
                         + sizeof(bool);

    public static ISerializer Create(JoinGamePacket packet)
    {
        return new JoinGameSerializer(
            packet.Entity,
            (byte) packet.GameMode,
            (byte) packet.Dimension,
            (byte) packet.Difficulty,
            packet.Players,
            packet.Type.ToString(),
            packet.ReducedDebugInformation);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(entity)
            .WriteByte(gameMode)
            .WriteByte(dimension)
            .WriteByte(difficulty)
            .WriteByte(players)
            .WriteVariableString(type)
            .WriteBoolean(reducedDebugInformation);
    }
}