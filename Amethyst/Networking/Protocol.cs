using System.Buffers;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Serializers;

namespace Amethyst.Networking;

internal static class Protocol
{
    public static bool TryRead(ref ReadOnlySequence<byte> sequence, out Packet packet)
    {
        var reader = new SequenceReader<byte>(sequence);
        packet = default;

        if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadVariableInteger(out var identifier))
        {
            return false;
        }

        var left = Variable.GetByteCount(identifier);

        if (!reader.TryReadExact(length - left, out var part))
        {
            return false;
        }

        packet = new Packet(identifier, part);
        sequence = sequence.Slice(part.End);

        return true;
    }

    // Both packets and serializers should be object pooled.
    public static int Write(Span<byte> span, IOutgoingPacket packet, ISerializer serializer)
    {
        var identifier = Variable.GetByteCount(packet.Identifier);

        var length = identifier + serializer.Length;
        var body = Variable.GetByteCount(length);

        SpanWriter
            .Create(span)
            .WriteVariableInteger(length)
            .WriteVariableInteger(packet.Identifier);

        var index = body + identifier;
        serializer.Write(span[index..]);

        return body + length;
    }
}