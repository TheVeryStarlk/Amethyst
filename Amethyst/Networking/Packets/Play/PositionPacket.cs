using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PositionPacket(Position position, bool ground) : IIngoingPacket<PositionPacket>, IProcessor
{
    public static State State => State.Play;

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

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;

        player.Position = position;
        player.Ground = ground;

        dispatcher.Dispatch(player, new Moved(position, player.Yaw, player.Pitch));
    }
}