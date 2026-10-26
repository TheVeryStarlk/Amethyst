using System.Diagnostics;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class TabRequestPacket(string behind) : IIngoingPacket<TabRequestPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 20;

    public static TabRequestPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new TabRequestPacket(reader.ReadVariableString());
    }

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var tab = dispatcher.Dispatch(client.Player, new Tab(behind));

        // This is not a complete implementation of the tab feature.
        // Only command names get their slash back, chat words are completed as they are.
        var command = tab.Behind.StartsWith('/') && !tab.Behind.Contains(' ');

        client.Write(command
            ? new TabResponsePacket(tab.Matches.Select(match => $"/{match}").ToArray())
            : new TabResponsePacket(tab.Matches));
    }
}