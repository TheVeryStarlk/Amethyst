using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Numerics;

namespace Amethyst.Protocol;

internal sealed class Transport(IDuplexPipe duplexPipe)
{
    public async Task<Message?> ReadAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await duplexPipe.Input.ReadAsync(cancellationToken);
            var buffer = result.Buffer;
            var consumed = buffer.Start;
            var examined = buffer.End;

            try
            {
                if (TryRead(ref buffer, out var message))
                {
                    consumed = buffer.Start;
                    examined = consumed;

                    return message;
                }

                if (result.IsCompleted)
                {
                    if (buffer.Length > 0)
                    {
                        // The message is incomplete and there's no more data to process.
                        throw new InvalidOperationException("Incomplete message.");
                    }

                    break;
                }
            }
            finally
            {
                duplexPipe.Input.AdvanceTo(consumed, examined);
            }
        }

        return null;

        static bool TryRead(ref ReadOnlySequence<byte> buffer, [NotNullWhen(true)] out Message? message)
        {
            var reader = new SequenceReader<byte>(buffer);
            message = null;

            if (!reader.TryReadVariableInteger(out var length) || !reader.TryReadVariableInteger(out var identifier))
            {
                return false;
            }

            var padding = VariableInteger.GetBytesCount(identifier);

            if (!reader.TryReadExact(length - padding, out var payload))
            {
                return false;
            }

            message = new Message(identifier, payload.ToArray());
            buffer = buffer.Slice(length + padding);
            return true;
        }
    }

    public async Task WriteAsync(IOutgoingPacket packet)
    {
        Write(duplexPipe.Output, packet);
        await duplexPipe.Output.FlushAsync();
        return;

        static void Write(PipeWriter writer, IOutgoingPacket packet)
        {
            var memory = writer.GetMemory(packet.CalculateLength());
            writer.Advance(Write(packet, memory));
            return;

            static int Write(IOutgoingPacket packet, Memory<byte> memory)
            {
                var packetWriter = new MemoryWriter(memory);
                packet.Write(ref packetWriter);

                var position = packetWriter.Position;
                var temporary = memory[..position].ToArray();
                var payloadWriter = new MemoryWriter(memory);

                payloadWriter.WriteVariableInteger(VariableInteger.GetBytesCount(packet.Identifier) + position);
                payloadWriter.WriteVariableInteger(packet.Identifier);
                payloadWriter.Write(temporary);

                return payloadWriter.Position;
            }
        }
    }
}

internal static class SequenceReaderExtensions
{
    public static bool TryReadVariableInteger(ref this SequenceReader<byte> reader, out int value)
    {
        var numbersRead = 0;
        var result = 0;

        byte read;

        do
        {
            if (!reader.TryRead(out read))
            {
                value = default;
                return false;
            }

            var temporaryValue = read & 0b01111111;
            result |= temporaryValue << 7 * numbersRead;

            numbersRead++;

            if (numbersRead <= 5)
            {
                continue;
            }

            value = default;
            return false;
        } while ((read & 0b10000000) != 0);

        value = result;
        return true;
    }
}

internal static class VariableInteger
{
    public static int GetBytesCount(int value)
    {
        return (BitOperations.LeadingZeroCount((uint) value | 1) - 38) * -1171 >> 13;
    }
}