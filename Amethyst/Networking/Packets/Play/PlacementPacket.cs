using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PlacementPacket(Position position, byte face) : IIngoingPacket<PlacementPacket>, IProcessor
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PlacementPacket(reader.ReadPosition(), reader.ReadByte());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {

    }
}