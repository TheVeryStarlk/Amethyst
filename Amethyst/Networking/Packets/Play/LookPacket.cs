namespace Amethyst.Networking.Packets.Play;

internal sealed class LookPacket(float yaw, float pitch, bool ground) : IIngoingPacket<LookPacket>
{
    public static int Identifier => 5;

    public float Yaw => yaw;

    public float Pitch => pitch;

    public bool Ground => ground;

    public static LookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LookPacket(reader.ReadFloat(), reader.ReadFloat(), reader.ReadBoolean());
    }
}