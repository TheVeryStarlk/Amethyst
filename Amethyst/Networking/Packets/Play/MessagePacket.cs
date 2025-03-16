namespace Amethyst.Networking.Packets.Play;

internal sealed record MessagePacket(string Message) : IIngoingPacket<MessagePacket>, IProcessor
{
    public static int Identifier => 1;

    public static MessagePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new MessagePacket(reader.ReadVariableString());
    }

    public void Process(Client client)
    {
        // Dispatch a send event.
    }
}