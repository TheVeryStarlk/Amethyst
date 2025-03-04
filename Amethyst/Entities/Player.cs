using System.Diagnostics;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Worlds;

namespace Amethyst.Entities;

[DebuggerDisplay("{Username}")]
internal sealed class Player(Client client, string username, World world) : IPlayer
{
    public IClient Client => client;

    public string Username => username;

    public IWorld World => world;

    public int Identifier => client.Identifier;

    public Location Location { get; set; }

    public Angle Yaw { get; set; }

    public Angle Pitch { get; set; }

    public bool OnGround { get; set; }

    public Guid Guid { get; } = Guid.NewGuid();

    public string? Locale { get; set; }

    public byte ViewDistance { get; set; }

    public void Teleport(Location location)
    {
        Location = location;
        Client.Write(new PositionLookPacket(location, Yaw, Pitch, OnGround));
    }

    public void Send(Message message, MessagePosition position = MessagePosition.Box)
    {
        Client.Write(new MessagePacket(message.Serialize(), (byte) position));
    }

    public void Disconnect(Message message)
    {
        Client.Stop(message);
    }
}