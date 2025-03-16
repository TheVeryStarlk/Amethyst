namespace Amethyst.Worlds;

internal static class NumericUtility
{
    public static int ToChunk(this int value)
    {
        return value >> 4;
    }

    public static long Encode(int first, int second)
    {
        return (long) second << 32 | (uint) first;
    }

    public static void Decode(this long value, out int first, out int second)
    {
        first = (int) (value & uint.MaxValue);
        second = (int) (value >> 32);
    }
}