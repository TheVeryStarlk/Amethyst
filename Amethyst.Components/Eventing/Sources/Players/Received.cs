using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Components.Eventing.Sources.Players;

public sealed class Received(Packet packet) : Event<IPlayer>
{
    /// <remarks>Using this property outside the <see cref="Received"/> event scope will result in a use-after-free exception.</remarks>
    public Packet Packet => packet;
}