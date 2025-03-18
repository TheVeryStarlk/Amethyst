using Amethyst.Abstractions;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Client;

public sealed record Login(string Username) : Event<IClient>
{
    public IWorld? World { get; set; }
}