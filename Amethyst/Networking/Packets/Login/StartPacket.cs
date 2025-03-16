namespace Amethyst.Networking.Packets.Login;

internal sealed record StartPacket(string Username) : IIngoingPacket<StartPacket>
{
    public static int Identifier => 0;

    public static StartPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StartPacket(reader.ReadVariableString());
    }
}