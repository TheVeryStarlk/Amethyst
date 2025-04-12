using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class MessagePacket(string message) : IIngoingPacket<MessagePacket>, IProcessor
{
    public static int Identifier => 1;

    public static MessagePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new MessagePacket(reader.ReadVariableString());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(client.Player!, new Sent(message));
    }
}