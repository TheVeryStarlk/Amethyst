using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities;

internal sealed record Player(int Identifier, IClient Client, string Unique, string Username, IWorld World) : IPlayer
{
    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public IWorld World { get; set; } = World;

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