using Amethyst.Entities;

namespace Amethyst.Protocol;

internal interface IHandler
{
    // Or what about passing the interface types instead.
    public void Handle(Server server, Player player, Client client);
}