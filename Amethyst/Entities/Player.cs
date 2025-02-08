using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username) : IPlayer
{
    public IClient Client => client;

    public string Username => username;

    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public void Disconnect(Message message)
    {
        client.Stop(message);
    }
}