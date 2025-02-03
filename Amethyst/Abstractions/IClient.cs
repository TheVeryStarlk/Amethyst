using Amethyst.Components.Messages;
using Amethyst.Protocol;

namespace Amethyst.Abstractions;

public interface IClient
{
    public void Stop(Message message);
    
    internal ValueTask WriteAsync(params IOutgoingPacket[] packets);
}