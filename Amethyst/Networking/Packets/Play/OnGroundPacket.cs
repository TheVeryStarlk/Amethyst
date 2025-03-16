namespace Amethyst.Networking.Packets.Play;

internal sealed record OnGroundPacket(bool OnGround) : IIngoingPacket<OnGroundPacket>, IProcessor
{
    public static int Identifier => 3;

    public static OnGroundPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new OnGroundPacket(reader.ReadBoolean());
    }

    public void Process(Client client)
    {
        client.Player!.OnGround = OnGround;
    }
}