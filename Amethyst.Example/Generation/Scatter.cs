namespace Amethyst.Example.Generation;

/// <summary>
/// Deterministic randomness keyed by coordinates, so the same seed always decorates the same spots.
/// </summary>
internal static class Scatter
{
    public static int Hash(int seed, int x, int z, int salt)
    {
        var hash = (uint) seed;

        hash = Mix(hash ^ (uint) x * 0x9E3779B1);
        hash = Mix(hash ^ (uint) z * 0x85EBCA77);
        hash = Mix(hash ^ (uint) salt * 0xC2B2AE3D);

        return (int) hash;
    }

    /// <returns>A value between 0 (inclusive) and 1 (exclusive).</returns>
    public static double Chance(int seed, int x, int z, int salt)
    {
        return (uint) Hash(seed, x, z, salt) / 4294967296.0;
    }

    private static uint Mix(uint value)
    {
        value ^= value >> 16;
        value *= 0x7FEB352D;
        value ^= value >> 15;
        value *= 0x846CA68B;
        value ^= value >> 16;

        return value;
    }
}
