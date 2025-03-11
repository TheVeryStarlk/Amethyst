using Amethyst.Abstractions.Networking;

namespace Amethyst.Abstractions;

public interface IClient
{
    public int Identifier { get; }

    public void Write(IOutgoingPacket packet);

    public void Stop();
}