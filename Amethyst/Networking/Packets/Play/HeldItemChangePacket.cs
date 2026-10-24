using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class HeldItemChangePacket(short index) : IIngoingPacket<HeldItemChangePacket>, IProcessor
{
    public static int Identifier => 9;

    public static HeldItemChangePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new HeldItemChangePacket(reader.ReadShort());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        if (index is < 0 or > 8)
        {
            return;
        }

        eventDispatcher.Dispatch(client.Player!, new Selected(index));
    }
}
