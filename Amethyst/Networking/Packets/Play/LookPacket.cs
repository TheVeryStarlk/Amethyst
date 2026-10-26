using System.Diagnostics;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class LookPacket(float yaw, float pitch, bool ground) : IIngoingPacket<LookPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 5;

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        var player = client.Player;

        player.Yaw = yaw;
        player.Pitch = pitch;
        player.Ground = ground;

        dispatcher.Dispatch(player, new Moved(player.Position, yaw, pitch));
    }
}