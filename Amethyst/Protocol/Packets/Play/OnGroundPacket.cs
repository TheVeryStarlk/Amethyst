namespace Amethyst.Protocol.Packets.Play;

internal sealed record OnGroundPacket(bool Value) : IIngoingPacket<OnGroundPacket>
{
    public static int Identifier => 3;

    public static OnGroundPacket Create(SpanReader reader)
    {
        return new OnGroundPacket(reader.ReadBoolean());
    }
}