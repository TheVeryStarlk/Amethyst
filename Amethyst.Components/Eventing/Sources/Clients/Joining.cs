using Amethyst.Components.Worlds;

namespace Amethyst.Components.Eventing.Sources.Clients;

public sealed class Joining : Event<IClient>
{
    public IWorld? World { get; set; }
}