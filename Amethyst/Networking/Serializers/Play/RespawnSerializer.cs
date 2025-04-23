using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class RespawnSerializer(Dimension dimension, Difficulty difficulty, GameMode gameMode, string type) : ISerializer<RespawnPacket, RespawnSerializer>
{
    public int Length => sizeof(int) + sizeof(byte) + sizeof(byte) + Variable.GetByteCount(type);

    public static RespawnSerializer Create(RespawnPacket packet)
    {
        return new RespawnSerializer(packet.World.Dimension, packet.World.Difficulty, packet.Player.GameMode, packet.World.Type.ToString());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteInteger((int) dimension)
            .WriteByte((byte) difficulty)
            .WriteByte((byte) gameMode)
            .WriteVariableString(type);
    }
}