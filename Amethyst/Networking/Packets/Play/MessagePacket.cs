using System.Diagnostics;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class MessagePacket(string message) : IIngoingPacket<MessagePacket>, IProcessor
{
    public static int Identifier => 1;

    public static MessagePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new MessagePacket(reader.ReadVariableString());
    }

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        dispatcher.Dispatch(client.Player, new Sent(message));
    }
}