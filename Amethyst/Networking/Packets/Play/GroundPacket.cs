using System.Diagnostics;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed partial class GroundPacket(bool value) : IIngoingPacket<GroundPacket>, IProcessor
{
    public static State State => State.Play;

    public static int Identifier => 3;

    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

        client.Player.Ground = value;
    }
}