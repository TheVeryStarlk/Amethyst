using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class JoinGameSerializer(
    int unique,
    byte gameMode,
    byte dimension,
    byte difficulty,
    byte players,
    string type,
    bool reducedDebugInformation) : ISerializer<JoinGamePacket, JoinGameSerializer>
{
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
}