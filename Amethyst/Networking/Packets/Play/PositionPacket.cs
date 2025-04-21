using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
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
        var difference = position - client.Player!.Position;

        if (difference is not { X: 0, Y: 0, Z: 0 })
        {
            var packet = new EntityRelativePositionLookPacket(client.Player.Unique, difference, client.Player.Yaw, client.Player.Pitch, ground);

            foreach (var pair in client.Player.World.Players.Where(pair => pair.Value != client.Player))
            {
                pair.Value.Client.Write(packet);
            }
        }

        eventDispatcher.Dispatch(client.Player!, new Moved(position, client.Player!.Yaw, client.Player.Pitch));
        client.Player!.Synchronize(position, client.Player.Yaw, client.Player.Pitch, ground);
    }
}