using Amethyst.Components.Messages;
using Amethyst.Protocol;

namespace Amethyst.Components;

public interface IClient
{
    public State State { get; }

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop(Message message);
}

public enum State
{
    Handshake,
    Status,
    Login,
    Play
}