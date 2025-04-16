using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class TimeSerializer(long age, long time) : ISerializer<TimePacket, TimeSerializer>
{
    public int Length => sizeof(long) * 2;

    public static TimeSerializer Create(TimePacket packet)
    {
        return new TimeSerializer(packet.Age, packet.Time);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteLong(age).WriteLong(time);
    }
}