using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class HeldSlotSerializer(byte slot) : ISerializer<HeldSlotPacket, HeldSlotSerializer>
{
    public static HeldSlotSerializer Create(HeldSlotPacket packet)
    {
        return new HeldSlotSerializer(packet.Slot);
    }
}