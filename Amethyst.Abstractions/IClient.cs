using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Protocol;

namespace Amethyst.Abstractions;

public interface IClient
{
    ValueTask WriteAsync(params IOutgoingPacket[] packets);

    public void Stop(Message reason);
}