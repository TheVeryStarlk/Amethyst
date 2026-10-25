using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities.Players;
using Amethyst.Worlds;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Login;

internal sealed class StartPacket(string username) : IIngoingPacket<StartPacket>, IProcessor
{
    public static int Identifier => 0;

    private readonly Message taken = Message.Simple("Username already taken!");
    
    public static StartPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StartPacket(reader.ReadVariableString());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var joining = eventDispatcher.Dispatch(client, new Joining(username));

        try
        {
            if (joining.IsCancelled)
            {
                client.Write(new FailurePacket(joining.Message));
                client.Stop();

                return;
            }

            var world = joining.World as World ?? throw new InvalidOperationException("No world was set.");
            var player = new Player(client, world, Guid.NewGuid(), joining.GameMode, joining.Username);

            // Usernames are only unique within a world ignoring case, unless UsernameSubscriber is registered.
            // The player is only assigned to the client once added, so a rejected player never dispatches a left event.
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
            eventDispatcher.Dispatch(player, new Joined());
        }
        finally
        {
            // Without a player the client never gets a left event, so subscribers that claimed something
            // on joining must hear about every other outcome, including a later subscriber cancelling it.
            if (client.Player is null)
            {
                eventDispatcher.Dispatch(client, new Rejected(joining.Username));
            }
        }
    }
}