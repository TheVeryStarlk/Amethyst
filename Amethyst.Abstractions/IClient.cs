using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Networking;

namespace Amethyst.Abstractions;

public interface IClient
{
    public int Identifier { get; }

    public IPlayer? Player { get; }

    /// <summary>
    /// Writes an <see cref="IOutgoingPacket"/> to the <see cref="IClient"/>.
    /// </summary>
    /// <remarks>
    /// Misusing this method might lead to weird behaviors and/or bugs.
    /// </remarks>
    /// <param name="packet">The <see cref="IOutgoingPacket"/> to write.</param>
    public void Write(IOutgoingPacket packet);

    /// <summary>
    /// Stops the <see cref="IClient"/>'s connection.
    /// </summary>
    /// <remarks>
    /// Use <see cref="IPlayer.Disconnect"/> for a graceful disconnection.
    /// </remarks>
    public void Stop();
}