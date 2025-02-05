namespace Amethyst.Protocol.Packets.Play;

public sealed record LookPacket(float Yaw, float Pitch, bool OnGround) : IIngoingPacket<LookPacket>
{
    public static int Identifier => 5;

    static LookPacket IIngoingPacket<LookPacket>.Create(SpanReader reader)
    {
        return new LookPacket(reader.ReadFloat(), reader.ReadFloat(), reader.ReadBoolean());
    }
}