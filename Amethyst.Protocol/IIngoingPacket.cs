using System.Buffers;

namespace Amethyst.Protocol;

public interface IIngoingPacket
{
    public static abstract int Identifier { get; }
}

public interface ICreatable<out T> where T : IIngoingPacket
{
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