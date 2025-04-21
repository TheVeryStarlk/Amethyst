using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PositionLookPacket(Position position, float yaw, float pitch, bool ground) : IIngoingPacket<PositionLookPacket>, IProcessor
{
    public static int Identifier => 6;

    public static PositionLookPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new PositionLookPacket(
            new Position(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),
            reader.ReadFloat(),
            reader.ReadFloat(),
            reader.ReadBoolean());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var difference = position - client.Player!.Position;
        var packet = new EntityRelativePositionLookPacket(client.Player.Unique, difference, (byte) client.Player.Yaw, (byte) client.Player.Pitch, ground);

        foreach (var pair in client.Player.World.Players.Where(pair => pair.Value != client.Player))
        {
            pair.Value.Client.Write(packet);
        }

        eventDispatcher.Dispatch(client.Player!, new Moved(position, yaw, pitch));
        client.Player!.Synchronize(position, yaw, pitch, ground);
    }
}