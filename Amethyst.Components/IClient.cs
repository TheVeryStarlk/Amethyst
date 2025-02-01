using Amethyst.Components.Messages;
using Amethyst.Protocol;

namespace Amethyst.Components;

public interface IClient
{
    ValueTask WriteAsync(params IOutgoingPacket[] packets);

    public void Stop(Message reason);
}