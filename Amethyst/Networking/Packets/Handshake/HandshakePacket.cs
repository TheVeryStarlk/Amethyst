using Amethyst.Abstractions.Packets.Login;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Client;

namespace Amethyst.Networking.Packets.Handshake;

internal sealed partial class HandshakePacket([Variable] int version, string address, ushort port, [Variable] State state) : IIngoingPacket<HandshakePacket>, IProcessor
{
    public static State State => State.Handshake;

    public static int Identifier => 0;

    public void Process(Client client, Dispatcher dispatcher)
    {
        if (state is not (State.Status or State.Login))
        {
            // Invalid state.
            client.Stop();
        }

        client.State = state;

        // The supported protocol version.
        if (state is State.Status || version is 47)
        {
            return;
        }

        var outdated = dispatcher.Dispatch(client, new Outdated());

        client.Write(new FailurePacket(outdated.Message));
        client.Stop();
    }
}