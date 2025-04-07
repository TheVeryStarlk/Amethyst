using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Entities;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Login;

namespace Amethyst.Networking.Processors;

internal sealed class LoginProcessor : IProcessor
{
    public static void Process(Client client, Packet packet)
    {
        var joining = client.EventDispatcher.Dispatch(client, new Joining(packet.Create<StartPacket>().Username));
        var world = joining.World ?? throw new InvalidOperationException("No world was set.");

        client.Player = new Player(client, Guid.NewGuid().ToString(), joining.GameMode, joining.Username, world);

        client.Write(
            new SuccessPacket(client.Player.Unique, client.Player.Username),
            new JoinGamePacket(client.Player.Identifier, client.Player.GameMode, world.Dimension, world.Difficulty, byte.MaxValue, world.Type, false),
            new PositionLookPacket(new Position(), 0, 0));

        client.State = State.Play;
        client.EventDispatcher.Dispatch(client.Player, new Joined());
    }
}