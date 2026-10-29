using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities.Players;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Login;
using Amethyst.Worlds;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class LoginProcessor(Dispatcher dispatcher) : IProcessor<StartPacket>
{
    private readonly Message taken = Message.Simple("Username already taken!");

    public void Process(Client client, StartPacket packet)
    {
        var joining = dispatcher.Dispatch(client, new Joining(packet.Username));

        if (joining.IsCancelled)
        {
            client.Write(new FailurePacket(joining.Message));
            client.Stop();

            return;
        }

        var world = joining.World as World ?? throw new InvalidOperationException("No world was set.");
        var player = new Player(client, world, Guid.NewGuid(), joining.GameMode, joining.Username);

        if (!world.TryAdd(player))
        {
            client.Write(new FailurePacket(taken));
            client.Stop();

            return;
        }

        client.Player = player;

        client.Write(
            new SuccessPacket(player),
            new JoinGamePacket(player, world, byte.MaxValue, false),
            new PositionLookPacket(new Position(), 0, 0));

        client.State = State.Play;
        dispatcher.Dispatch(player, new Joined());
    }
}