using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

internal sealed class TabRequestPacket : IIngoingPacket<TabRequestPacket>, IDispatchable
{
    public static int Identifier => 20;

    public required string Behind { get; init; }

    public static TabRequestPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new TabRequestPacket
        {
            Behind = reader.ReadVariableString()
        };
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        var tab = eventDispatcher.Dispatch(player, new Tab(Behind));

        // Needs more work. IDK if having logic here is "good" or not.
        player.Client.Write(new TabCompletePacket(tab.Matches));
    }
}