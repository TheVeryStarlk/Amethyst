namespace Amethyst.Protocol.Packets.Play;

internal sealed record LookPacket(float Yaw, float Pitch, bool OnGround) : LookPacketBase(Yaw, Pitch, OnGround), ICreatable<LookPacketBase>
{
    public static LookPacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LookPacket(reader.ReadFloat(), reader.ReadFloat(), reader.ReadBoolean());
    }
}