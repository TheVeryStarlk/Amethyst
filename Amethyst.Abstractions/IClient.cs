using Amethyst.Abstractions.Messages;
using Amethyst.Protocol;

namespace Amethyst.Abstractions;

public interface IClient
{
    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop(Message message);
}