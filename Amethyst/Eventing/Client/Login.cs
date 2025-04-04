using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Client;

public sealed class Login(string username) : Event<IClient>
{
    public string Username => username;

    public IWorld? World { get; set; }

    public GameMode GameMode { get; set; }

    public Location Location { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }
}