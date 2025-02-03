using System.Numerics;
using System.Text;

namespace Amethyst.Abstractions.Protocol;

internal static class Variable
{
    public static int GetByteCount(int value)
    {
        return (BitOperations.LeadingZeroCount((uint) value | 1) - 38) * -1171 >> 13;
    }

    public static int GetByteCount(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        return GetByteCount(length) + length;
    }
}