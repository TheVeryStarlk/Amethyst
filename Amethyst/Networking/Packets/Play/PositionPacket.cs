using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PositionPacket(Position position, bool ground) : IIngoingPacket<PositionPacket>, IProcessor
{
    public static int Identifier => 4;

    public static PositionPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionPacket(
            new Position(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadBoolean());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        eventDispatcher.Dispatch(client.Player!, new Moved(position, client.Player!.Yaw, client.Player.Pitch));
        client.Player!.Synchronize(position, client.Player.Yaw, client.Player.Pitch, ground);
    }
}