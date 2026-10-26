using System.Diagnostics;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class HeldItemChangePacket(short index) : IIngoingPacket<HeldItemChangePacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 9;

    public static HeldItemChangePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new HeldItemChangePacket(reader.ReadShort());
    }

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        if (index is < 0 or > 8)
        {
            return;
        }

        dispatcher.Dispatch(client.Player, new Selected(index));
    }
}