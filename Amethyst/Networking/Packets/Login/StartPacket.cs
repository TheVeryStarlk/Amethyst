namespace Amethyst.Networking.Packets.Login;

internal sealed class StartPacket(string username) : IIngoingPacket<StartPacket>
{
    public string Username => username;

    public static int Identifier => 0;

    public static StartPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StartPacket(reader.ReadVariableString());
    }
}