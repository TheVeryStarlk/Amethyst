using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Client;

// Find a better name for this.
public sealed class Login(string username) : Event<IClient>
{
    public string Username => username;

    public IWorld? World { get; set; }

    public GameMode GameMode { get; set; }
}