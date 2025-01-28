using System.Buffers;

namespace Amethyst.Protocol;

public interface IIngoingPacket<out T> where T : IIngoingPacket<T>
{
    public static abstract int Identifier { get; }

    internal static abstract T Create(SpanReader reader);
}

public readonly struct Packet(int identifier, ReadOnlySequence<byte> sequence)
{
    public int Identifier => identifier;

    public void Out<T>(out T packet) where T : IIngoingPacket<T>
    {
        if (T.Identifier != identifier)
        {
            throw new InvalidOperationException("Unexpected packet identifier.");
        }

        var reader = new SpanReader(sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray());
        packet = T.Create(reader);
    }
}