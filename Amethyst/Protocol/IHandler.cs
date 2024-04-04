using Amethyst.Entities;

namespace Amethyst.Protocol;

internal interface IHandler
{
    // Or what about passing the interface types instead.
    public Task HandleAsync(Server server, Player player, Client client);
}