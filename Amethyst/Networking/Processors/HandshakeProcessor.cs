using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Eventing.Client;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Handshake;

namespace Amethyst.Networking.Processors;

internal sealed class HandshakeProcessor : IProcessor
{
    public static void Process(Client client, Packet packet)
    {
        var handshake = packet.Create<HandshakePacket>();

        if (handshake.Version != 47)
        {
            var outdated = client.EventDispatcher.Dispatch(client, new Outdated());

            client.Write(new FailurePacket(outdated.Message));
            client.Stop();
        }

        client.State = (State) handshake.State;

        if (client.State is State.Login or State.Status)
        {
            return;
        }

        // Invalid state.
        client.Stop();
    }
}