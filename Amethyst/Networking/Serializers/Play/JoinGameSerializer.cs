using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class JoinGameSerializer(
    int unique,
    byte gameMode,
    byte dimension,
    byte difficulty,
    byte players,
    string type,
    bool reducedDebugInformation) : ISerializer<JoinGamePacket, JoinGameSerializer>
{
    public int Length => sizeof(int) + sizeof(byte) * 4 + Variable.GetByteCount(type) + sizeof(bool);

    public static JoinGameSerializer Create(JoinGamePacket packet)
    {
        return new JoinGameSerializer(
            packet.Player.Unique,
            (byte) packet.Player.GameMode,
            (byte) packet.World.Dimension,
            (byte) packet.World.Difficulty,
            packet.List,
            packet.World.Type.ToString(),
            packet.ReducedDebugInformation);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger(unique)
            .WriteByte(gameMode)
            .WriteByte(dimension)
            .WriteByte(difficulty)
            .WriteByte(players)
            .WriteVariableString(type)
            .WriteBoolean(reducedDebugInformation);
    }
}