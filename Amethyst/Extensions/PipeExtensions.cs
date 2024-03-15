using System.Buffers;
using System.Diagnostics;
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
        writer.Advance(Write(packet, writer.GetMemory()));
        await writer.FlushAsync();
        return;

        static int Write(IOutgoingPacket packet, Memory<byte> memory)
        {
            var writer = new MemoryWriter(memory);

            var packetLength = packet.CalculateLength();
            writer.WriteVariableInteger(VariableInteger.GetBytesCount(packet.Identifier) + packetLength);
            writer.WriteVariableInteger(packet.Identifier);

            var old = writer.Position;
            packet.Write(ref writer);
            Debug.Assert(writer.Position - old == packetLength);

            return writer.Position;
        }
    }
}