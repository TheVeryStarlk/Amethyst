using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;

namespace Amethyst.Components;

public interface IClient
{
    public int Identifier { get; }

    /// <summary>
    /// Writes packet(s) to the client.
    /// </summary>
    /// <remarks>
    /// Misusing this method might result in crashes or weird behaviors.
    /// </remarks>
    /// <param name="packets">The packet(s) to write.</param>
    public void Write(params ReadOnlySpan<IOutgoingPacket> packets);

    public void Stop(Message message);
}