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

    public static StartPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StartPacket(reader.ReadVariableString());
    }

    private static readonly Message taken = Message.Simple("Username already taken!");

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        // Usernames are unique across the whole server, so subscribers never see a duplicate join.
        if (client.WorldFactory.Worlds.Any(world => world.Players.ContainsKey(username)))
        {
            Reject(taken);
            return;
        }

        var joining = eventDispatcher.Dispatch(client, new Joining(username));

        if (joining.IsCancelled)
        {
            Reject(joining.Message);
            return;
        }

        var world = joining.World as World ?? throw new InvalidOperationException("No world was set.");
        var player = new Player(client, Guid.NewGuid(), joining.GameMode, joining.Username, world);

        // Catches a concurrent join with the same username that slipped past the check above.
        // The player is only assigned to the client once added, so a rejected player never dispatches a left event.
        if (!world.TryAdd(player))
        {
            Reject(taken);
            return;
        }

        client.Player = player;

        client.Write(
            new SuccessPacket(player),
            new JoinGamePacket(player, world, byte.MaxValue, false),
            new PositionLookPacket(new Position(), 0, 0));

        client.State = State.Play;
        eventDispatcher.Dispatch(player, new Joined());

        return;

        void Reject(Message message)
        {
            client.Write(new FailurePacket(message));
            client.Stop();
        }
    }
}