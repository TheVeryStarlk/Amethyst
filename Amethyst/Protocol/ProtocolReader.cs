using System.Buffers;
using System.IO.Pipelines;

namespace Amethyst.Protocol;

internal class ProtocolReader(PipeReader input)
{
    private SequencePosition consumed;
    private SequencePosition examined;
    private bool read;

    public async ValueTask<Packet> ReadAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await input.ReadAsync(cancellationToken).ConfigureAwait(false);
            var buffer = result.Buffer;

            // In the event that no packet is parsed successfully,
            // mark consumed as nothing and examined as the entire buffer.
            consumed = buffer.Start;
            examined = buffer.End;

            if (TryRead(ref buffer, out var packet))
            {
                // A single packet was successfully parsed so mark the start of the parsed buffer as consumed.
                // TryRead trims the buffer to point to the data after the packet was parsed.
                consumed = buffer.Start;

                // Examined is marked the same as consumed here,
                // so the next call to ReadAsync will process the next packet if there's one.
                examined = consumed;

                read = true;

                return packet;
            }

            input.AdvanceTo(consumed, examined);

            // Reset the state since we're done consuming this buffer.
            consumed = default;
            examined = default;

            // There's no more data to be processed.
            if (result.IsCompleted)
            {
                throw new OperationCanceledException();
            }
        }

        static bool TryRead(ref ReadOnlySequence<byte> buffer, out Packet packet)
        {
            var reader = new SequenceReader<byte>(buffer);
            packet = default;

            if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadVariableInteger(out var identifier))
            {
                return false;
            }

            var left = Variable.GetByteCount(identifier);

            if (!reader.TryReadExact(length - left, out var sequence))
            {
                return false;
            }

            packet = new Packet(identifier, sequence);
            buffer = buffer.Slice(sequence.End);

            return true;
        }
    }

    public void Advance()
    {
        if (!read)
        {
            return;
        }

        input.AdvanceTo(consumed, examined);
        read = false;
    }
}