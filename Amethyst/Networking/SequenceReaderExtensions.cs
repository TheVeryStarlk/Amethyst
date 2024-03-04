using System.Buffers;

namespace Amethyst.Networking;

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