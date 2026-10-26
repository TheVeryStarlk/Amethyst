using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class SpawnPaintingSerializer([Variable] int unique, string motive, Position position, byte direction) : ISerializer<SpawnPaintingPacket, SpawnPaintingSerializer>
{
    public static SpawnPaintingSerializer Create(SpawnPaintingPacket packet)
    {
        return new SpawnPaintingSerializer(
            packet.Painting.Unique,
            packet.Painting.Motive.ToString(),
            packet.Painting.Position,
            (byte) packet.Painting.Direction);
    }
}