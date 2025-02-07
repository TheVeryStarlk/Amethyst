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
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous write operation.</returns>
    public ValueTask WriteAsync(params IOutgoingPacket[] packets);

    public void Stop(Message message);
}