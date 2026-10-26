using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class SingleChunkSerializer(int x, int z, bool continuous, ushort bitmask, byte[] sections) : ISerializer<SingleChunkPacket, SingleChunkSerializer>
{
    public static SingleChunkSerializer Create(SingleChunkPacket packet)
    {
        // Chunks are always sent whole, along with their biomes.
        return new SingleChunkSerializer(packet.X, packet.Z, true, packet.Bitmask, packet.Sections);
    }
}