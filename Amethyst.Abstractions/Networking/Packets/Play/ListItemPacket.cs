using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

// This does not support multiple players.
public sealed class ListItemPacket(IListItemAction action, IPlayer player) : IOutgoingPacket
{
    public int Identifier => 56;

    public IListItemAction Action => action;

    public IPlayer Player => player;
}

// Could this just be an enum?
public interface IListItemAction
{
    public int Identifier { get; }
}

public sealed class AddPlayerAction(string username, GameMode gameMode, int latency, Message tag) : IListItemAction
{
    public int Identifier => 0;

    public string Username => username;

    public GameMode GameMode => gameMode;

    public int Latency => latency;

    public string Tag => tag.Serialize();
}

public sealed class RemovePlayerAction : IListItemAction
{
    public int Identifier => 4;
}