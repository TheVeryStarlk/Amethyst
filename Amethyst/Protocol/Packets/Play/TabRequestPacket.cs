using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Protocol.Packets.Play;

internal sealed record TabRequestPacket(string Behind) : IIngoingPacket<TabRequestPacket>, IDispatchable
{
    public static int Identifier => 20;

    public static TabRequestPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new TabRequestPacket(reader.ReadVariableString());
    }

    public void Dispatch(Player player, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(player, new Tab(Behind));
    }
}