using Amethyst.Components.Worlds;

namespace Amethyst.Components.Eventing.Sources.Clients;

public sealed class Joining(string username) : Event<IClient>
{
    public string Username => username;

    public IWorld? World { get; set; }
}