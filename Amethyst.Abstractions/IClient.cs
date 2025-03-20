using Amethyst.Abstractions.Networking.Packets;

namespace Amethyst.Abstractions;

public interface IClient
{
    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop();
}