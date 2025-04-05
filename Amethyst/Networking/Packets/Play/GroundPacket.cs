namespace Amethyst.Networking.Packets.Play;

internal sealed class GroundPacket(bool value) : IIngoingPacket<GroundPacket>
{
    public static int Identifier => 3;

    public bool Value => value;

    public static GroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new GroundPacket(reader.ReadBoolean());
    }
}