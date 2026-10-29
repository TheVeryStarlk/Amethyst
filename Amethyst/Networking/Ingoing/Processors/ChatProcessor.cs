using System.Diagnostics;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Play;
using TabResponsePacket = Amethyst.Abstractions.Packets.Play.TabResponsePacket;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class ChatProcessor(Dispatcher dispatcher) : IProcessor<MessagePacket>, IProcessor<TabRequestPacket>
{
    public void Process(Client client, MessagePacket packet)
    {
        Debug.Assert(client.Player is not null);

        dispatcher.Dispatch(client.Player, new Sent(packet.Message));
    }

    public void Process(Client client, TabRequestPacket packet)
    {
        Debug.Assert(client.Player is not null);

        var tab = dispatcher.Dispatch(client.Player, new Tab(packet.Behind));

        // This is not a complete implementation of the tab feature.
        // Only command names get their slash back, chat words are completed as they are.
        var command = tab.Behind.StartsWith('/') && !tab.Behind.Contains(' ');

        client.Write(command
            ? new TabResponsePacket(tab.Matches.Select(match => $"/{match}").ToArray())
            : new TabResponsePacket(tab.Matches));
    }
}