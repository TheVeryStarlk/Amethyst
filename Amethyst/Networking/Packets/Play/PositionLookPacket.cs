using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class PositionLookPacket(double x, double y, double z, float yaw, float pitch, bool ground) : IIngoingPacket<PositionLookPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 6;

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;
        var position = new Position(x, y, z);

        player.Position = position;
        player.Yaw = yaw;
        player.Pitch = pitch;
        player.Ground = ground;

        dispatcher.Dispatch(player, new Moved(position, yaw, pitch));
    }
}