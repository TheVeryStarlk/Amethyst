using System.Buffers;

namespace Amethyst.Networking.Packets;

internal interface IIngoingPacket<out T> where T : IIngoingPacket<T>
{
    public static abstract int Identifier { get; }

    public static abstract T Create(ReadOnlySpan<byte> span);
}

internal readonly struct Packet(int identifier, ReadOnlySequence<byte> sequence)
{
    public int Identifier => identifier;

    // Create an object pool for this.
    public T Create<T>() where T : IIngoingPacket<T>
    {
        if (T.Identifier != identifier)
        {
            throw new InvalidOperationException("Unexpected packet identifier.");
        }

        return T.Create(sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray());
    }
}