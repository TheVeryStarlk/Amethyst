using System.Numerics;

namespace Amethyst.Utilities;

internal static class VariableInteger
{
    public static int GetBytesCount(int value)
    {
        return (BitOperations.LeadingZeroCount((uint) value | 1) - 38) * -1171 >> 13;
    }
}