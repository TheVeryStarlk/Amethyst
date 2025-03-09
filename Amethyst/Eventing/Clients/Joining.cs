using Amethyst.Abstractions;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Clients;

public sealed class Joining(string username) : Event<IClient>
{
    public string Username => username;

    public IWorld? World { get; set; }
}