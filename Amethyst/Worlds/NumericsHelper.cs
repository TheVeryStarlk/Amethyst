namespace Amethyst.Worlds;

internal static class NumericsHelper
{
    public static long Encode(int first, int second)
    {
        return (long) second << 32 | (uint) first;
    }
}