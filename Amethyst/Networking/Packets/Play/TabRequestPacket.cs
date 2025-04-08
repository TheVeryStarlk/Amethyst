namespace Amethyst.Networking.Packets.Play;

internal sealed class TabRequestPacket(string behind) : IIngoingPacket<TabRequestPacket>
{
    public static int Identifier => 20;

    public string Behind => behind;

    public static TabRequestPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new TabRequestPacket(reader.ReadVariableString());
    }
}