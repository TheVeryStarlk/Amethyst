using System.Buffers;

namespace Amethyst.Networking;

internal static class SequenceReaderExtensions
{
    public static bool TryReadVariableInteger(ref this SequenceReader<byte> reader, out int value)
    {
        var result = 0;
        var read = 0;

        byte current;

        do
        {
            if (!reader.TryRead(out current))
            {
                value = 0;
                return false;
            }

            var temporaryValue = current & 0b01111111;
            result |= temporaryValue << 7 * read;

            read++;

            if (read <= 5)
            {
                continue;
            }

            value = 0;
            return false;
        } while ((current & 0b10000000) != 0);

        value = result;
        return true;
    }
}