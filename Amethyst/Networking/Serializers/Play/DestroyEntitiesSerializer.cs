using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class DestroyEntitiesSerializer([Variable] int[] uniques) : ISerializer<DestroyEntitiesPacket, DestroyEntitiesSerializer>
{
    public static DestroyEntitiesSerializer Create(DestroyEntitiesPacket packet)
    {
        return new DestroyEntitiesSerializer(packet.Entities.Select(entity => entity.Unique).ToArray());
    }
}