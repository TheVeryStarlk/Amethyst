using System.Buffers;

namespace Amethyst.Networking.Ingoing.Packets;

internal interface IIngoingPacket<out T> where T : IIngoingPacket<T>
{

    public static abstract T Create(ReadOnlySpan<byte> span);
}

internal readonly struct Packet(int identifier, ReadOnlySequence<byte> sequence)
{
    public int Identifier => identifier;

    // Create an object pool for this.
    public T Create<T>() where T : IIngoingPacket<T>
    {

        return T.Create(sequence.IsSingleSegment ? sequence.FirstSpan : sequence.ToArray());
    }
}