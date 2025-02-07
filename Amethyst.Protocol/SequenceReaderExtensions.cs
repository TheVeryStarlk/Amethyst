using System.Buffers;

namespace Amethyst.Protocol;

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
                value = 0;
                return false;
            }

            var temporaryValue = read & 0b01111111;
            result |= temporaryValue << 7 * numbersRead;

            numbersRead++;

            if (numbersRead <= 5)
            {
                continue;
            }

            value = 0;
            return false;
        } while ((read & 0b10000000) != 0);

        value = result;
        return true;
    }
}