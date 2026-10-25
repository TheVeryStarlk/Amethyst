using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class HeldSlotSerializer(byte slot) : ISerializer<HeldSlotPacket, HeldSlotSerializer>
{
    public int Length => sizeof(byte);

    public static HeldSlotSerializer Create(HeldSlotPacket packet)
    {
        return new HeldSlotSerializer(packet.Slot);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteByte(slot);
    }
}