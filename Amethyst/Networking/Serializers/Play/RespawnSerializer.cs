using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class RespawnSerializer(int dimension, Difficulty difficulty, GameMode gameMode, string type) : ISerializer<RespawnPacket, RespawnSerializer>
{
    public static RespawnSerializer Create(RespawnPacket packet)
    {
        // Unlike joining, the dimension is sent as an integer.
        return new RespawnSerializer((int) packet.World.Dimension, packet.World.Difficulty, packet.Player.GameMode, packet.World.Type.ToString());
    }
}