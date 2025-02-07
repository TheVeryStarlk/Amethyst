using Amethyst.Abstractions.Messages;
using Amethyst.Protocol;

namespace Amethyst.Abstractions;

public interface IClient
{
    /// <summary>
    /// Writes packet(s) to the client.
    /// </summary>
    /// <remarks>
    /// This is intended to be internally used in Amethyst.
    /// </remarks>
    /// <param name="packets">The packet(s) to write.</param>
    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop(Message message);
}