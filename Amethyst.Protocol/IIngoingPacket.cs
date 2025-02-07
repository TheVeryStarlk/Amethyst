using System.Buffers;

namespace Amethyst.Protocol;

public interface IIngoingPacket
{
    public static abstract int Identifier { get; }
}

/// <summary>
/// Represents a creatable <see cref="IIngoingPacket"/>.
/// </summary>
/// <remarks>
/// This is intended to be internally used in Amethyst.
/// </remarks>
/// <typeparam name="T">The packet's type.</typeparam>
public interface ICreatable<out T> where T : IIngoingPacket
{
    /// <summary>
    /// Creates a new instance of <see cref="T"/> from a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> used for creation.</param>
    /// <returns>A new instance of <see cref="T"/>.</returns>
    public static abstract T Create(ReadOnlySpan<byte> span);
}

public readonly struct Packet(int identifier, ReadOnlySequence<byte> sequence)
{
    public int Identifier => identifier;

    public T Create<T>() where T : IIngoingPacket, ICreatable<T>
    {
        if (T.Identifier != identifier)
        {
            throw new InvalidOperationException("Unexpected packet identifier.");
        }

        return T.Create(sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray());
    }
}