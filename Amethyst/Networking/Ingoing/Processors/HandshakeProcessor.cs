using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Handshake;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class HandshakeProcessor(Dispatcher dispatcher) : IProcessor<HandshakePacket>
{
    public void Process(Client client, HandshakePacket packet)
    {
        if (packet.State is not (State.Status or State.Login))
        {
            // Invalid state.
            client.Stop();
        }

        client.State = packet.State;

        // The supported protocol version.
        if (packet.State is State.Status || packet.Version is 47)
        {
            return;
        }

        var outdated = dispatcher.Dispatch(client, new Outdated());

        client.Write(new FailurePacket(outdated.Message));
        client.Stop();
    }
}