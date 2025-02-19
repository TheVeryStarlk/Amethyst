using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;
using Amethyst.Components.Worlds;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username, IWorld world) : IPlayer
{
    public IClient Client => client;

    public string Username => username;

    public IWorld World => world;

    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

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