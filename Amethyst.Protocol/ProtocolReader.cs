using System.Buffers;
using System.IO.Pipelines;

namespace Amethyst.Protocol;

public sealed class ProtocolReader(PipeReader input)
{
    private SequencePosition consumed;
    private SequencePosition examined;

    public async ValueTask<Message> ReadAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await input.ReadAsync(cancellationToken);
            var buffer = result.Buffer;

            // In the event that no message is parsed successfully,
            // mark consumed as nothing and examined as the entire buffer.
            consumed = buffer.Start;
            examined = buffer.End;

            if (TryRead(ref buffer, out var message))
            {
                // A single message was successfully parsed so mark the start of the parsed buffer as consumed.
                // TryRead trims the buffer to point to the data after the message was parsed.
                consumed = buffer.Start;

                // Examined is marked the same as consumed here,
                // so the next call to ReadAsync will process the next message if there's one.
                examined = consumed;

                return message;
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

        static bool TryRead(ref ReadOnlySequence<byte> buffer, out Message message)
        {
            var reader = new SequenceReader<byte>(buffer);
            message = default;

            if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadVariableInteger(out var identifier))
            {
                return false;
            }

            var left = Variable.GetByteCount(identifier);

            if (!reader.TryReadExact(length - left, out var sequence))
            {
                return false;
            }

            message = new Message(identifier, sequence);
            buffer = buffer.Slice(sequence.End);

            return true;
        }
    }

    public void Advance()
    {
        input.AdvanceTo(consumed, examined);
    }
}