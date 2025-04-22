using Amethyst.Abstractions.Packets;

namespace Amethyst.Abstractions;

public interface IClient
{
    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    /// <summary>
    /// Signals the stopping of the connection, all subsequent calls of <see cref="Write"/> will not be processed.
    /// Once all pending <see cref="IOutgoingPacket"/> packets have been sent, the underlying connection will close.
    /// </summary>
    public void Stop();
}