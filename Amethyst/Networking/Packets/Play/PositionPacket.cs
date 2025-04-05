using Amethyst.Abstractions.Entities;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PositionPacket(Location location, bool ground) : IIngoingPacket<PositionPacket>
{
    public static int Identifier => 4;

    public Location Location => location;

    public bool Ground => ground;

    public static PositionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionPacket(
            new Location(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadBoolean());
    }
}