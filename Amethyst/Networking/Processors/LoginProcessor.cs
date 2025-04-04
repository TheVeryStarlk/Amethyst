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
        var start = packet.Create<StartPacket>();
        var login = client.EventDispatcher.Dispatch(client, new Login(start.Username));

        if (login.World is { } world)
        {
            client.Player = new Player(client, Guid.NewGuid().ToString(), start.Username, world);

            client.Write(
                new SuccessPacket(client.Player.Unique, client.Player.Username),
                new JoinGamePacket(client.Player.Identifier, login.GameMode, world.Dimension, world.Difficulty, byte.MaxValue, world.Type, false),
                new PositionLookPacket(new Location(), 0, 0));

            client.State = State.Play;
            client.EventDispatcher.Dispatch(client.Player, new Joined());
        }
    }
}