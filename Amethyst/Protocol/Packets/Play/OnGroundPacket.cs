namespace Amethyst.Protocol.Packets.Play;

internal sealed record OnGroundPacket(bool Value) : OnGroundPacketBase(Value), ICreatable<OnGroundPacketBase>
{
    public static OnGroundPacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new OnGroundPacket(reader.ReadBoolean());
    }
}