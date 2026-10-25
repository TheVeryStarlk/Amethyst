using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

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

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;

        player.Position = position;
        player.Yaw = yaw;
        player.Pitch = pitch;
        player.Ground = ground;

        dispatcher.Dispatch(player, new Moved(position, yaw, pitch));
    }
}