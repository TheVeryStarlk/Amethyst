using Playground.Abstractions.Networking;

namespace Playground.Abstractions;

public interface IClient
{
    public int Identifier { get; }

    public void Write(IOutgoingPacket packet);

    public void Stop();
}