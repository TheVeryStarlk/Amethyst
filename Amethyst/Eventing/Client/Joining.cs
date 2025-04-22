using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Eventing.Client;

public sealed class Joining(string username) : IEvent<IClient>
{
    public string Username => username;

    public IWorld? World { get; set; }

    public GameMode GameMode { get; set; }
}