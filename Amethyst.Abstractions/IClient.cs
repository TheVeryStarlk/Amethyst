using Amethyst.Abstractions.Networking.Packets;

namespace Amethyst.Abstractions;

public interface IClient
{
    public int Identifier { get; }

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop();
}