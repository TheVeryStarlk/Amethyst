namespace Amethyst.Networking.Packets.Play;

internal sealed record TabRequestPacket(string Behind) : IIngoingPacket<TabRequestPacket>
{
    public static int Identifier => 20;

    public static TabRequestPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new TabRequestPacket(reader.ReadVariableString());
    }
}