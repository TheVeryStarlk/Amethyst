using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal sealed class Player(IClient client, string unique, GameMode gameMode, string username, IWorld world) : Entity, IPlayer
{
    public IClient Client => client;

    public IWorld World { get; set; } = world;

    public string Unique => unique;

    public GameMode GameMode { get; set; } = gameMode;

    public string Username => username;

    public string? Locale { get; set; }

    public byte ViewDistance { get; set; }

    public void Send(Message message, MessagePosition position = MessagePosition.Box)
    {
        Client.Write(new MessagePacket(message, position));
    }

    public void Disconnect(Message message)
    {
        Client.Write(new DisconnectPacket(message));
        Client.Stop();
    }
}