using Amethyst.Components.Protocol;

namespace Amethyst.Components.Eventing.Sources.Clients;

public sealed class Received(Packet packet) : Event<IClient>
{
    /// <remarks>Using this property outside the <see cref="Received"/> event scope will result in a use-after-free exception.</remarks>
    public Packet Packet => packet;
}