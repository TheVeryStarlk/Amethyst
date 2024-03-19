using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using Amethyst.Networking;
using Amethyst.Utilities;

namespace Amethyst.Extensions;

internal static class PipeExtensions
{
    public static async Task<Message?> ReadMessageAsync(this PipeReader reader, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await reader.ReadAsync(cancellationToken);
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
                        throw new InvalidDataException("Incomplete message.");
                    }

                    break;
                }
            }
            finally
            {
                reader.AdvanceTo(consumed, examined);
            }
        }

        return null;

        static bool TryRead(ref ReadOnlySequence<byte> buffer, [NotNullWhen(true)] out Message? message)
        {
            var reader = new SequenceReader<byte>(buffer);
            message = null;

            if (!reader.TryReadVariableInteger(out var length)
                || !reader.TryReadVariableInteger(out var identifier))
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

    public static async Task WritePacketAsync(this PipeWriter writer, IOutgoingPacket packet)
    {
        writer.QueuePacket(packet);
        await writer.FlushAsync();
    }

    public static void QueuePacket(this PipeWriter writer, IOutgoingPacket packet)
    {
        var memory = writer.GetMemory(short.MaxValue);
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