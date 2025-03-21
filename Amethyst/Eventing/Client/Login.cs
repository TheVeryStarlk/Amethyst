using Amethyst.Abstractions;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Client;

public sealed class Login(string username) : Event<IClient>
{
    public string Username => username;

    public IWorld? World { get; set; }
}