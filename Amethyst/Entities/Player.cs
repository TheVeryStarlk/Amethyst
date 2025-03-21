using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal sealed class Player(IClient client, string unique, string username, IWorld world) : IPlayer
{
    public IClient Client => client;

    public string Unique => unique;

    public string Username => username;

    // Probably shouldn't use random.
    public int Identifier { get; } = Random.Shared.Next();

    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public IWorld World { get; set; } = world;

    public GameMode GameMode { get; set; }

    public string? Locale { get; set; }

    public byte ViewDistance { get; set; }

    public void Send(Message message, MessagePosition position = MessagePosition.Box)
    {
        Client.Write(new MessagePacket(message, position));
    }

    public void Disconnect(Message message)
    {
        // We trust the client to actually disconnect.
        Client.Write(new DisconnectPacket(message));
    }
}