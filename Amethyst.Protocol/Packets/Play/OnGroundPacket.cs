namespace Amethyst.Protocol.Packets.Play;

public sealed record OnGroundPacket(bool Value) : IIngoingPacket<OnGroundPacket>
{
    public static int Identifier => 3;

    static OnGroundPacket IIngoingPacket<OnGroundPacket>.Create(SpanReader reader)
    {
        return new OnGroundPacket(reader.ReadBoolean());
    }
}