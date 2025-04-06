using Amethyst.Abstractions.Entities;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PositionPacket(Position position, bool ground) : IIngoingPacket<PositionPacket>
{
    public static int Identifier => 4;

    public Position Position => position;

    public bool Ground => ground;

    public static PositionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionPacket(
            new Position(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadBoolean());
    }
}