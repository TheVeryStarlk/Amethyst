using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class TimeSerializer(long age, long time) : ISerializer<TimePacket, TimeSerializer>
{
    public static TimeSerializer Create(TimePacket packet)
    {
        return new TimeSerializer(packet.Age, packet.Time);
    }
}