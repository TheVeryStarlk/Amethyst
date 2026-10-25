using System.Buffers;
using System.IO.Compression;
using Amethyst.Abstractions.Packets;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Serializers;

namespace Amethyst.Networking;

// Once compression is enabled, the packet's length is followed by the uncompressed length of the identifier and body,
// which is zero if they were left uncompressed for being below the threshold.
internal static class Protocol
{
    // The largest uncompressed length the vanilla server accepts.
    private const int Maximum = 2097152;

    public static bool TryRead(ref ReadOnlySequence<byte> sequence, int threshold, out Packet packet)
    {
        var reader = new SequenceReader<byte>(sequence);
        packet = default;

        if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadExact(length, out var part))
        {
            return false;
        }

        packet = threshold < 0 ? Read(part) : Decompress(part, threshold);
        sequence = sequence.Slice(part.End);

        return true;
    }

    // Both packets and serializers should be object pooled.
    public static void Write(IBufferWriter<byte> writer, IOutgoingPacket packet, int threshold)
    {
        var serializer = packet.Create();

        var identifier = Variable.GetByteCount(packet.Identifier);
        var length = identifier + serializer.Length;

        if (threshold < 0)
        {
            var body = Variable.GetByteCount(length);
            var span = writer.GetSpan(body + length);

            SpanWriter
                .Create(span)
                .WriteVariableInteger(length)
                .WriteVariableInteger(packet.Identifier);

            serializer.Write(span[(body + identifier)..]);
            writer.Advance(body + length);

            return;
        }

        if (length < threshold)
        {
            // The zero uncompressed length takes a single byte.
            var body = Variable.GetByteCount(length + 1);
            var span = writer.GetSpan(body + length + 1);

            SpanWriter
                .Create(span)
                .WriteVariableInteger(length + 1)
                .WriteVariableInteger(0)
                .WriteVariableInteger(packet.Identifier);

            serializer.Write(span[(body + 1 + identifier)..]);
            writer.Advance(body + length + 1);

            return;
        }

        Compress(writer, packet, serializer, identifier, length);
    }

    private static Packet Read(ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);

        if (!reader.TryReadVariableInteger(out var identifier))
        {
            throw new InvalidOperationException("Packet is missing its identifier.");
        }

        return new Packet(identifier, sequence.Slice(reader.Position));
    }

    private static Packet Decompress(ReadOnlySequence<byte> sequence, int threshold)
    {
        var reader = new SequenceReader<byte>(sequence);

        if (!reader.TryReadVariableInteger(out var length))
        {
            throw new InvalidOperationException("Packet is missing its uncompressed length.");
        }

        if (length is 0)
        {
            return Read(sequence.Slice(reader.Position));
        }

        if (length < threshold || length > Maximum)
        {
            throw new InvalidOperationException("Packet has an invalid uncompressed length.");
        }

        var buffer = new byte[length];

        using (var stream = new ZLibStream(new MemoryStream(sequence.Slice(reader.Position).ToArray()), CompressionMode.Decompress))
        {
            stream.ReadExactly(buffer);
        }

        return Read(new ReadOnlySequence<byte>(buffer));
    }

    private static void Compress(IBufferWriter<byte> writer, IOutgoingPacket packet, ISerializer serializer, int identifier, int length)
    {
        var rented = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            SpanWriter.Create(rented).WriteVariableInteger(packet.Identifier);
            serializer.Write(rented.AsSpan(identifier, length - identifier));

            using var compressed = new MemoryStream();

            using (var stream = new ZLibStream(compressed, CompressionLevel.Fastest, leaveOpen: true))
            {
                stream.Write(rented, 0, length);
            }

            var total = Variable.GetByteCount(length) + (int) compressed.Length;
            var body = Variable.GetByteCount(total);

            var span = writer.GetSpan(body + total);

            SpanWriter
                .Create(span)
                .WriteVariableInteger(total)
                .WriteVariableInteger(length)
                .Write(compressed.GetBuffer().AsSpan(0, (int) compressed.Length));

            writer.Advance(body + total);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }
}