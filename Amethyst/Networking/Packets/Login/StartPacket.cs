using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Login;

internal sealed class StartPacket(string username) : IIngoingPacket<StartPacket>, IProcessor
{
    public static int Identifier => 0;

    public static StartPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new StartPacket(reader.ReadVariableString());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var joining = eventDispatcher.Dispatch(client, new Joining(username));
        var world = joining.World ?? throw new InvalidOperationException("No world was set.");

        client.Player = new Player(client, Guid.NewGuid().ToString(), joining.GameMode, joining.Username, world);

        client.Write(
            new SuccessPacket(client.Player.Unique, client.Player.Username),
            new JoinGamePacket(client.Player.Identifier, client.Player.GameMode, world.Dimension, world.Difficulty, byte.MaxValue, world.Type, false),
            new PositionLookPacket(new Position(), 0, 0));

        client.State = State.Play;
        eventDispatcher.Dispatch(client.Player, new Joined());
    }
}