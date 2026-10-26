using System.Diagnostics;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class MessagePacket(string message) : IIngoingPacket<MessagePacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 1;

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        dispatcher.Dispatch(client.Player, new Sent(message));
    }
}