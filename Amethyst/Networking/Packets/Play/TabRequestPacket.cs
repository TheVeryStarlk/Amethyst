using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class TabRequestPacket(string behind) : IIngoingPacket<TabRequestPacket>, IProcessor
{
    public static int Identifier => 20;

    public static TabRequestPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new TabRequestPacket(reader.ReadVariableString());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var tab = eventDispatcher.Dispatch(client.Player!, new Tab(behind));

        // This is not a complete implementation of the tab feature.
        client.Write(tab.Behind.Contains(' ')
            ? new TabResponsePacket(tab.Matches)
            : new TabResponsePacket(tab.Matches.Select(match => $"/{match}").ToArray()));
    }
}