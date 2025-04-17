using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
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

        client.Player = new Player(client, Guid.NewGuid(), joining.GameMode, joining.Username, world);

        client.Write(
            new SuccessPacket(client.Player),
            new JoinGamePacket(client.Player, world, byte.MaxValue, false),
            new PositionLookPacket(new Position(), 0, 0));

        client.State = State.Play;
        eventDispatcher.Dispatch(client.Player, new Joined());
    }
}