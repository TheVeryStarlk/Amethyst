using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using Amethyst.Networking.Packets;

namespace Amethyst.Networking;

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

            var padding = VariableIntegerHelper.GetBytesCount(identifier);

            if (!reader.TryReadExact(length - padding, out var payload))
            {
                return false;
            }

            message = new Message(identifier, payload.ToArray());
            buffer = buffer.Slice(length + padding);
            return true;
        }
    }

    public static async Task WritePacketAsync(this PipeWriter writer, IOutgoingPacket packet, CancellationToken token)
    {
        writer.Advance(Write(packet, writer.GetMemory()));
        await writer.FlushAsync(token);
        return;

        static int Write(IOutgoingPacket packet, Memory<byte> memory)
        {
            var writer = new MemoryWriter(memory);
            writer.WriteVariableInteger(VariableIntegerHelper.GetBytesCount(packet.Identifier) + packet.CalculateLength());
            writer.WriteVariableInteger(packet.Identifier);
            packet.Write(ref writer);
            return writer.Position;
        }
    }
}

internal sealed record Message(int Identifier, Memory<byte> Memory)
{
    public T As<T>() where T : IIngoingPacket<T>
    {
        if (T.Identifier != Identifier)
        {
            throw new ArgumentException($"Expected {T.Identifier} but got {Identifier} instead.");
        }

        var reader = new MemoryReader(Memory);
        return T.Read(reader);
    }
}