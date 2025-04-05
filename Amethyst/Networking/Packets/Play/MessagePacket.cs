namespace Amethyst.Networking.Packets.Play;

internal sealed class MessagePacket(string message) : IIngoingPacket<MessagePacket>
{
    public static int Identifier => 1;

    public string Message => message;

    public static MessagePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new MessagePacket(reader.ReadVariableString());
    }
}