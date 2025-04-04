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
            client.EventDispatcher.Dispatch(client, new Outdated());
        }

        if (handshake.State > 2)
        {
            client.Stop();
        }

        client.State = (State) handshake.State;
    }
}