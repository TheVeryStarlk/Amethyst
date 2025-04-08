using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Eventing.Client;

namespace Amethyst.Networking.Packets.Handshake;

internal sealed class HandshakePacket(int version, string address, ushort port, State state) : IIngoingPacket<HandshakePacket>, IProcessor
{
    public static int Identifier => 0;

    public int Version => version;

    public string Address => address;

    public ushort Port => port;

    public State State => state;

    public static HandshakePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new HandshakePacket(
            reader.ReadVariableInteger(),
            reader.ReadVariableString(),
            reader.ReadUnsignedShort(),
            (State) reader.ReadVariableInteger());
    }

    public void Process(Client client)
    {
        if (state is not (State.Status or State.Login))
        {
            // Invalid state.
            client.Stop();
        }

        client.State = State;

        // The supported protocol version.
        if (state is State.Status || version is 47)
        {
            return;
        }

        var outdated = client.EventDispatcher.Dispatch(client, new Outdated());

        client.Write(new FailurePacket(outdated.Message));
        client.Stop();
    }
}