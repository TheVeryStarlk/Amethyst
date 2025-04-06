using Amethyst.Abstractions.Entities;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PositionLookPacket(Position position, float yaw, float pitch, bool ground) : IIngoingPacket<PositionLookPacket>
{
    public static int Identifier => 6;

    public Position Position => position;

    public float Yaw => yaw;

    public float Pitch => pitch;

    public bool Ground => ground;

    public static PositionLookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionLookPacket(
            new Position(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadBoolean());
    }
}