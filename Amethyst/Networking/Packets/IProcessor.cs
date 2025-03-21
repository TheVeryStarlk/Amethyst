using Amethyst.Entities;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets;

internal interface IProcessor
{
    public void Process(Player player, EventDispatcher eventDispatcher);
}