using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class PositionPacket(double x, double y, double z, bool ground) : IIngoingPacket<PositionPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 4;

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;
        var position = new Position(x, y, z);

        player.Position = position;
        player.Ground = ground;

        dispatcher.Dispatch(player, new Moved(position, player.Yaw, player.Pitch));
    }
}