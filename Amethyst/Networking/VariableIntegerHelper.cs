using System.Numerics;

namespace Amethyst.Networking;

internal static class VariableIntegerHelper
{
    public static int GetBytesCount(int value)
    {
        return (BitOperations.LeadingZeroCount((uint) value | 1) - 38) * -1171 >> 13;
    }
}