namespace Amethyst.Networking.Ingoing.Packets.Play;

internal sealed class HeldItemChangePacket(short index) : IIngoingPacket<HeldItemChangePacket>
{
    public const int Identifier = 9;

    public short Index => index;

    public static HeldItemChangePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new HeldItemChangePacket(reader.ReadShort());
    }
}