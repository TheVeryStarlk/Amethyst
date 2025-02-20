using System.Buffers;

namespace Amethyst.Abstractions.Protocol;

public interface IIngoingPacket<out T> where T : IIngoingPacket<T>
{
    public static abstract int Identifier { get; }

    public static abstract T Create(ReadOnlySpan<byte> span);
}

public readonly struct Packet(int identifier, ReadOnlySequence<byte> sequence)
{
    public int Identifier => identifier;

    public T Create<T>() where T : IIngoingPacket<T>
    {
        if (T.Identifier != identifier)
        {
            throw new InvalidOperationException("Unexpected packet identifier.");
        }

        return T.Create(sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray());
    }
}