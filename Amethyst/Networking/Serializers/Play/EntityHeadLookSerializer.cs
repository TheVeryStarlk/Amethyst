using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class EntityHeadLookSerializer([Variable] int unique, byte yaw) : ISerializer<EntityHeadLookPacket, EntityHeadLookSerializer>
{
    public static EntityHeadLookSerializer Create(EntityHeadLookPacket packet)
    {
        // Angles are in 1/256 of a full turn, going through an integer first so negative ones wrap instead of saturating.
        return new EntityHeadLookSerializer(packet.Entity.Unique, (byte) (int) (packet.Entity.Yaw / 360F * 256F));
    }
}