using Amethyst.Protocol;

namespace Amethyst.Components;

public interface IClient
{
    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop(string message);
}