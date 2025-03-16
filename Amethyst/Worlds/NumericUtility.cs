namespace Amethyst.Worlds;

internal static class NumericUtility
{
    public static int AsIndex(int x, int y, int z)
    {
        return (y & 0xF) << 8 | (z & 0xF) << 4 | x & 0xF;
    }

    public static int AsIndex(int x, int z)
    {
        return (z & 0xF) * 16 + (x & 0xF);
    }

    public static long Encode(int first, int second)
    {
        return (long) second << 32 | (uint) first;
    }

    public static void Decode(long value, out int first, out int second)
    {
        first = (int) (value & uint.MaxValue);
        second = (int) (value >> 32);
    }
}